using Net.Codecrete.QrCodeGenerator;
using qr.Shared;
using SixLabors.ImageSharp;

namespace qr.Application;

public class QrCodeSaver
{
    public void Save(QrCode qrCode, OutputFormat outputFormat, int border, Stream stream)
    {
        if (outputFormat == OutputFormat.Svg)
        {
            var svg = qrCode.ToSvgString(border);
            var writer = new StreamWriter(stream);
            writer.Write(svg);
            writer.Flush();
        }
        else if (outputFormat == OutputFormat.Png)
        {
            using var image = qrCode.ToImage(1, border, Color.Black, Color.White);

            image.SaveAsPng(stream);
        }
    }
}