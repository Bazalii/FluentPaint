using SkiaSharp;

namespace FluentPaint.Core.Converters.Implementations;

public class YCbCr709Converter : IConverter
{
    /// <summary>
    /// Essential constants for conversion formulas.
    /// </summary>
    private const float Kr = 0.2126f;
    private const float Kg = 0.7152f;
    private const float Kb = 0.0722f;

    /// <summary>
    /// Provides methods to convert picture from RGB to YCbCr709.
    /// </summary>
    /// <param name="bitmap"> bitmap containing all pixels of the picture.</param>
    /// <returns> converted bitmap.</returns>
    public SKBitmap FromRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var luminance = Kr * pixel.Red + Kg * pixel.Green + Kb * pixel.Blue;
                var blueComponent = pixel.Blue - luminance;
                var redComponent = pixel.Red - luminance;

                convertedBitmap.SetPixel(x, y,
                    new SKColor(
                        (byte) luminance,
                        (byte) ((236 + blueComponent) * 255 / 472),
                        (byte) ((200 + redComponent) * 255 / 400)
                    ));
            }
        }

        return convertedBitmap;
    }

    /// <summary>
    /// Provides methods to convert picture from YCbCr709 to RGB.
    /// </summary>
    /// <param name="bitmap"> bitmap containing all pixels of the picture.</param>
    /// <returns> converted bitmap.</returns>
    public SKBitmap ToRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
        
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var luminance = pixel.Red;
                var blueComponent = pixel.Green * 472 / 255 - 236;
                var redComponent = pixel.Blue * 400 / 255 - 200;

                var red = luminance + redComponent;
                var green = luminance - (Kr * redComponent + Kb * blueComponent) / Kg;
                var blue = luminance + blueComponent;

                if (red < 0)
                {
                    red = 0;
                }

                if (green < 0)
                {
                    green = 0;
                }

                if (blue < 0)
                {
                    blue = 0;
                }

                convertedBitmap.SetPixel(x, y, new SKColor((byte) red, (byte) green, (byte) blue));
            }
        }

        return convertedBitmap;
    }
}