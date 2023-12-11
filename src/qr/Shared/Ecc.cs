using Net.Codecrete.QrCodeGenerator;

namespace qr.Shared;

public enum EccMode
{
    Low,
    Medium,
    High,
    Quartile
}

public static class EccMapper
{
    public static QrCode.Ecc From(EccMode eccMode)
    {
        return eccMode switch
        {
            EccMode.Low => QrCode.Ecc.Low,
            EccMode.Medium => QrCode.Ecc.Medium,
            EccMode.High => QrCode.Ecc.High,
            EccMode.Quartile => QrCode.Ecc.Quartile,
            _ => throw new ArgumentException($"Unsupported ECL value '{eccMode.ToString()}'")
        };
    }
}