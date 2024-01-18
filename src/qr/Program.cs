using System.CommandLine;
using System.CommandLine.Invocation;
using Net.Codecrete.QrCodeGenerator;
using qr.Application;
using qr.Commands;
using qr.Shared;
using Spectre.Console;

var wifiCommand = EncodeWifiCommand.Create();

var rootCommand = new RootCommand("Create QR code. Enter text to encode interactively or provide it through standard input");
rootCommand.AddCommand(wifiCommand);

var eccOption = OptionsFactory.CreateEccOption();
var borderOption = OptionsFactory.CreateBorderOption();
var outputPathOption = OptionsFactory.CreateOutputPathOption();
var outputFormatOption = OptionsFactory.CreateOutputFormatOption();

rootCommand.AddOption(eccOption);
rootCommand.AddOption(borderOption);
rootCommand.AddOption(outputPathOption);
rootCommand.AddOption(outputFormatOption);

rootCommand.SetHandler((eccMode, border, outputPath, outputFormat, ctx) =>
    {
        try
        {
            string? text;
            if (ctx.Console.IsInputRedirected)
            {
                text = Console.In.ReadLine();
                if (string.IsNullOrWhiteSpace(text))
                {
                    throw new ArgumentException("Cannot read text to encode from stdin");
                }
            }
            else
            {
                text = AnsiConsole.Prompt(new TextPrompt<string>("Enter text to encode:"));
            }

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
    }, eccOption, borderOption, outputPathOption, outputFormatOption,
    Bind.FromServiceProvider<InvocationContext>());


return rootCommand.Invoke(args);
