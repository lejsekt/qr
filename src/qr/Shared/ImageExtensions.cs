using Net.Codecrete.QrCodeGenerator;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace qr.Shared;

public static class ImageExtensions
{
    public static Image ToImage(this QrCode qrCode, int scale, int border, Color foreground, Color background)
    {
        if (scale <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), "Value out of range");
        }
        if (border < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(border), "Value out of range");
        }

        var size = qrCode.Size;
        var dim = (size + border * 2) * scale;

        if (dim > short.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale or border too large");
        }

        var image = new Image<Rgb24>(dim, dim);

        image.Mutate(img =>
        {
            img.Fill(background);

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    if (qrCode.GetModule(x, y))
                    {
                        img.Fill(foreground, new Rectangle((x + border) * scale, (y + border) * scale, scale, scale));
                    }
                }
            }
        });

        return image;
    }
}