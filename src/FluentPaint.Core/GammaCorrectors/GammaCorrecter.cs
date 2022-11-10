using SkiaSharp;

namespace FluentPaint.Core.GammaCorrectors;

public class GammaCorrecter
{
    public SKBitmap ToNewGamma(SKBitmap bitmap, float gamma)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var redComponent = Math.Pow(pixel.Red / 255f, gamma);
                var greenComponent = Math.Pow(pixel.Green / 255f, gamma);
                var blueComponent = Math.Pow(pixel.Blue / 255f, gamma);

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
}