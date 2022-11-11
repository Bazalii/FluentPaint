using SkiaSharp;

namespace FluentPaint.Core.GammaCorrectors;

public class GammaCorrecter
{
    public SKBitmap ToNewGamma(SKBitmap bitmap, float gamma)
    {
        return gamma == 0 ? ToSRGB(bitmap) : Gamma(bitmap, gamma);
    }
    
    private SKBitmap Gamma(SKBitmap bitmap, float gamma)
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
                
                
                redComponent = Math.Pow(redComponent, gamma);
                greenComponent = Math.Pow(greenComponent, gamma);
                blueComponent = Math.Pow(blueComponent, gamma);

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

    private SKBitmap ToSRGB(SKBitmap bitmap)
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

                redComponent = redComponent <= 0.0031308f
                    ? redComponent * 12.92f
                    : Math.Pow(redComponent, 1.0f / 2.4f) * 1.055f - 0.055f;
                greenComponent = greenComponent <= 0.0031308f
                    ? greenComponent * 12.92f
                    : Math.Pow(greenComponent, 1.0f / 2.4f) * 1.055f - 0.055f;
                blueComponent = blueComponent <= 0.0031308f
                    ? blueComponent * 12.92f
                    : Math.Pow(blueComponent, 1.0f / 2.4f) * 1.055f - 0.055f;

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