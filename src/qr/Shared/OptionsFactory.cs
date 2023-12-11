using System.CommandLine;

namespace qr.Shared;

public static class OptionsFactory
{
    public static Option<EccMode> CreateEccOption()
    {
        var option = new Option<EccMode>("--ecl", "Error correction level");
        option.AddAlias("-e");
        option.SetDefaultValue(EccMode.Medium);
        return option;
    }

    public static Option<int> CreateBorderOption()
    {
        var option = new Option<int>("--border", "Border thickness around QR code");
        option.AddAlias("-b");
        option.SetDefaultValue(4);
        return option;
    }


}