using System.CommandLine;
using System.CommandLine.Invocation;
using Net.Codecrete.QrCodeGenerator;
using qr.Application;
using qr.Shared;

namespace qr.Commands;

public static class EncodeTextCommand
{
    public static Command Create()
    {
        var command = new Command("text", "Encode text to QR code. SVG output will be printed to stdout.");

        var textArgument = new Argument<string>("Text", "Text to encode");
        var eccOption = OptionsFactory.CreateEccOption();
        var borderOption = OptionsFactory.CreateBorderOption();
        var outputPathOption = OptionsFactory.CreateOutputPathOption();
        var outputFormatOption = OptionsFactory.CreateOutputFormatOption();

        command.AddArgument(textArgument);
        command.AddOption(eccOption);
        command.AddOption(borderOption);
        command.AddOption(outputPathOption);
        command.AddOption(outputFormatOption);
        command.SetHandler((text, eccMode, border, outputPath, outputFormat, ctx) =>
            {
                try
                {
                    var ecc = EccMapper.From(eccMode);
                    var qr = QrCode.EncodeText(text, ecc);
                    var saver = new QrCodeSaver();
                    using var stream = string.IsNullOrWhiteSpace(outputPath)
                        ? Console.OpenStandardOutput()
                        : File.OpenWrite(outputPath);

                    saver.Save(qr, outputFormat, border, stream);
                }
                catch (Exception ex)
                {
                    ctx.Console.WriteLine($"Failed to create QR code: {ex.Message}");
                    ctx.ExitCode = 1;
                }
            }, textArgument, eccOption, borderOption, outputPathOption, outputFormatOption,
            Bind.FromServiceProvider<InvocationContext>());
        return command;
    }
}