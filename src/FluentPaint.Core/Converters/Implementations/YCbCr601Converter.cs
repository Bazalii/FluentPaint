using SkiaSharp;

namespace FluentPaint.Core.Converters.Implementations;

public class YCbCr601Converter : IConverter
{
    private const float Kr = 0.299f;
    private const float Kg = 0.587f;
    private const float Kb = 0.114f;

    public SKBitmap FromRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                // Another variant (don't remove pls)
                // var luminance = 16 + 219 * Kr * red + 219 * Kg * green + 219 * Kb * blue;
                // var blueComponent = 128 - 112 * Kr / (1 - Kb) * red - 112 * Kg / (1 - Kb) * green + 112 * blue;
                // var redComponent = 128 + 112 * red - 112 * Kg / (1 - Kr) * green - 112 * Kb / (1 - Kr) * blue;

                var luminance = Kr * pixel.Red + Kg * pixel.Green + Kb * pixel.Blue;
                var blueComponent = pixel.Blue - luminance;
                var redComponent = pixel.Red - luminance;

                convertedBitmap.SetPixel(x, y,
                    new SKColor((byte) luminance,
                        (byte) ((225 + blueComponent) * 255 / 450),
                        (byte) ((178 + redComponent) * 255 / 356)));
            }
        }

        return convertedBitmap;
    }

    public SKBitmap ToRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var luminance = pixel.Red;
                var blueComponent = pixel.Green * 450 / 255 - 225;
                var redComponent = pixel.Blue * 356 / 255 - 178;

                // Another variant (don't remove pls)
                // var red = 298.082f * luminance + 408.583f * redComponent - 222.921;
                // var green = 298.082f * luminance - 100.291f * blueComponent - 208.120f * redComponent + 135.576;
                // var blue = 298.082f * luminance + 516.412f * blueComponent - 276.836;

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