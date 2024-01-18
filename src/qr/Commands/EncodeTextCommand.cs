using System.CommandLine;
using System.CommandLine.Invocation;
using Net.Codecrete.QrCodeGenerator;
using qr.Shared;
using SixLabors.ImageSharp;

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

                    if (outputFormat == OutputFormat.Svg)
                    {
                        var svg = qr.ToSvgString(border);

                        if (string.IsNullOrEmpty(outputPath))
                        {
                            ctx.Console.WriteLine(svg);
                        }
                        else
                        {
                            File.WriteAllText(outputPath, svg);
                        }
                    }
                    else if (outputFormat == OutputFormat.Png)
                    {
                        using var image = qr.ToImage(1, border, Color.Black, Color.White);

                        if (string.IsNullOrEmpty(outputPath))
                        {
                            using var stream = Console.OpenStandardOutput();
                            image.SaveAsPng(stream);
                        }
                        else
                        {
                            image.SaveAsPng(outputPath);
                        }
                    }
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