using SkiaSharp;

namespace FluentPaint.Core.GammaCorrectors;

public class GammaCorrecter
{
    private delegate double ConvertComponent(double value);
    private float _gamma;

    public SKBitmap ToNewGamma(SKBitmap bitmap, float gamma)
    {
        _gamma = gamma;

        var result = gamma switch
        {
            0 => ConvertToGamma(bitmap, ChangeComponentToSRGB),
            _ => ConvertToGamma(bitmap, ChangeComponentToGamma)
        };

        return result;
    }

    private SKBitmap ConvertToGamma(SKBitmap bitmap, ConvertComponent convertComponent)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var redComponent = pixel.Red / 255d;
                var greenComponent = pixel.Green / 255d;
                var blueComponent = pixel.Blue / 255d;

                redComponent = convertComponent(redComponent);
                greenComponent = convertComponent(greenComponent);
                blueComponent = convertComponent(blueComponent);

                convertedBitmap.SetPixel(x, y,
                    new SKColor(
                        (byte) (redComponent * 255),
                        (byte) (greenComponent * 255),
                        (byte) (blueComponent * 255)
                    ));
            }
        }

        return convertedBitmap;
    }

    private double ChangeComponentToGamma(double value)
    {
        return Math.Pow(value, _gamma);
    }

    private double ChangeComponentToSRGB(double value)
    {
        return value <= 0.0031308
            ? value * 12.92
            : Math.Pow(value, 1.0 / 2.4) * 1.055 - 0.055;
    }
}