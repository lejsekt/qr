using System.CommandLine;
using System.CommandLine.Invocation;
using Net.Codecrete.QrCodeGenerator;
using qr.Shared;
using Spectre.Console;

namespace qr.Commands;

public static class EncodeWifiCommand
{
    public static Command Create()
    {
        var command = new Command("wifi", "Encode WiFi information to QR code. SVG output will be printed to stdout.");

        var ssidArgument = new Argument<string>("SSID (WiFi name)");

        var encryptionTypeOption = new Option<EncryptionType>("--encryption", "WiFi encryption type");
        encryptionTypeOption.SetDefaultValue(EncryptionType.Wpa);
        encryptionTypeOption.IsRequired = false;

        var hiddenOption = new Option<bool>("--hidden", "Is WiFi hidden");
        hiddenOption.SetDefaultValue(false);
        hiddenOption.IsRequired = false;

        var eccOption = OptionsFactory.CreateEccOption();
        var borderOption = OptionsFactory.CreateBorderOption();
        var outputPathOption = OptionsFactory.CreateOutputPathOption();

        command.AddArgument(ssidArgument);
        command.AddOption(encryptionTypeOption);
        command.AddOption(hiddenOption);
        command.AddOption(eccOption);
        command.AddOption(borderOption);
        command.AddOption(outputPathOption);

        command.SetHandler((ssid, encryptionType, isHidden, eccMode, border, outputPath, ctx) =>
            {
                try
                {
                    var encryption = encryptionType switch
                    {
                        EncryptionType.Wep => "WEP",
                        EncryptionType.Wpa => "WPA",
                        EncryptionType.Empty => string.Empty,
                        _ => throw new ArgumentException($"$Unsupported encryption type '{encryptionType.ToString()}'")
                    };

                    string encryptionPasswordSegment;
                    if (encryption == string.Empty)
                    {
                        encryptionPasswordSegment = "T:nopass;P:;";
                    }
                    else
                    {
                        string password;
                        if (ctx.Console.IsInputRedirected)
                        {
                            var passwordInput = Console.In.ReadLine();
                            if (string.IsNullOrWhiteSpace(passwordInput))
                            {
                                throw new ArgumentException("Cannot read WIFI password from stdin");
                            }

                            password = passwordInput;
                        }
                        else
                        {
                            password = AnsiConsole.Prompt(new TextPrompt<string>("Enter WiFi password:").Secret('*'));
                        }

                        encryptionPasswordSegment = $"T:{encryption};P:{password};";
                    }

                    var hiddenSegment = isHidden ? "H:true;" : string.Empty;

                    var text = $"WIFI:S:{ssid};{encryptionPasswordSegment}{hiddenSegment};";

                    var ecc = EccMapper.From(eccMode);
                    var qr = QrCode.EncodeText(text, ecc);
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
                catch (Exception ex)
                {
                    ctx.Console.WriteLine($"Failed to create QR code: {ex.Message}");
                    ctx.ExitCode = 1;
                }
            }, ssidArgument, encryptionTypeOption, hiddenOption, eccOption, borderOption, outputPathOption,
            Bind.FromServiceProvider<InvocationContext>());

        return command;
    }

    internal enum EncryptionType
    {
        Wep,
        Wpa,
        Empty
    }
}