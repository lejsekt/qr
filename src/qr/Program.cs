using System.CommandLine;
using qr.Commands;

var rootCommand = EncodeTextCommand.Create();

return rootCommand.Invoke(args);
