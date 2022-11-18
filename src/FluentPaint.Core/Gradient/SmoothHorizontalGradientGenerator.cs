using SkiaSharp;

namespace FluentPaint.Core.Gradient;

public class SmoothHorizontalGradientGenerator
{
    public SKBitmap CreateGradient(bool isRed, bool isGreen, bool isBlue)
    {
        var bitmap = new SKBitmap(256, 50);
        var red = 0;
        var green = 0;
        var blue = 0;

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                if (isRed)
                {
                    red = x;
                }

                if (isGreen)
                {
                    green = x;
                }

                if (isBlue)
                {
                    blue = x;
                }

                bitmap.SetPixel(x, y, new SKColor((byte) red, (byte) green, (byte) blue));
            }
        }

        return bitmap;
    }
}