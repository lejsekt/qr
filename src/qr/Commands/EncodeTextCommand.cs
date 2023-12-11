using System.CommandLine;
using System.CommandLine.Invocation;
using Net.Codecrete.QrCodeGenerator;
using qr.Shared;

namespace qr.Commands;

public static class EncodeTextCommand
{
    public static Command Create()
    {
        var rootCommand = new Command("text","Encode text to QR code. SVG output will be printed to stdout.");

        var textArgument = new Argument<string>("Text", "Text to encode");
        var eccOption = OptionsFactory.CreateEccOption();
        var borderOption = OptionsFactory.CreateBorderOption();

        rootCommand.AddArgument(textArgument);
        rootCommand.AddOption(eccOption);
        rootCommand.AddOption(borderOption);
        rootCommand.SetHandler((text, eccMode, border, ctx) =>
        {
            try
            {
                var ecc = EccMapper.From(eccMode);
                var qr = QrCode.EncodeText(text, ecc);
                var svg = qr.ToSvgString(border);
                ctx.Console.WriteLine(svg);
            }
            catch (Exception ex)
            {
                ctx.Console.WriteLine($"Failed to create QR code: {ex.Message}");
                ctx.ExitCode = 1;
            }
        }, textArgument, eccOption, borderOption, Bind.FromServiceProvider<InvocationContext>());
        return rootCommand;
    }
}