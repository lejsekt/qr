using System.CommandLine;
using System.CommandLine.Invocation;
using Net.Codecrete.QrCodeGenerator;
using qr.Shared;

var rootCommand = new RootCommand("Create QR code. SVG output will be printed to stdout.");

var textArgument = new Argument<string>("Text", "Text to encode");

var eccOption = new Option<EccMode>("--ecl", "Error correction level");
eccOption.AddAlias("-e");
eccOption.SetDefaultValue(EccMode.Medium);

var borderOption = new Option<int>("--border", "Border thickness around QR code");
borderOption.AddAlias("-m");
borderOption.SetDefaultValue(4);

rootCommand.AddArgument(textArgument);
rootCommand.AddOption(eccOption);
rootCommand.AddOption(borderOption);
rootCommand.SetHandler((text, eccMode, border, ctx) =>
{
    try
    {
        var ecc = eccMode switch
        {
            EccMode.Low => QrCode.Ecc.Low,
            EccMode.Medium => QrCode.Ecc.Medium,
            EccMode.High => QrCode.Ecc.High,
            EccMode.Quartile => QrCode.Ecc.Quartile,
            _ => throw new ArgumentException($"Unsupported ECL value '{eccMode.ToString()}'")
        };
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

return rootCommand.Invoke(args);

internal enum EccMode
{
    Low,
    Medium,
    High,
    Quartile
}