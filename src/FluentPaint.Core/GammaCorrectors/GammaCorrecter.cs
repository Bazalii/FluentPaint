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

                var redComponent = Math.Pow(pixel.Red, 1 / gamma);
                var greenComponent = Math.Pow(pixel.Green, 1 / gamma);
                var blueComponent = Math.Pow(pixel.Blue, 1 / gamma);

                convertedBitmap
                    .SetPixel(x, y, new SKColor((byte) redComponent, (byte) greenComponent, (byte) blueComponent));
            }
        }

        return convertedBitmap;
    }
}