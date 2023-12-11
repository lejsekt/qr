using System.CommandLine;
using qr.Commands;

var textCommand = EncodeTextCommand.Create();
var wifiCommand = EncodeWifiCommand.Create();

var rootCommand = new RootCommand();
rootCommand.AddCommand(textCommand);
rootCommand.AddCommand(wifiCommand);

return rootCommand.Invoke(args);
