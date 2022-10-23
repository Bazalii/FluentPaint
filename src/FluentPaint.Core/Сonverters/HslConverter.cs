using SkiaSharp;

namespace FluentPaint.Core.Сonverters;

public class HslConverter : IConverter
{
    public SKBitmap FromRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap();

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var currentPixel = bitmap.GetPixel(x, y);

                var red = currentPixel.Red / 255.0f;
                var green = currentPixel.Green / 255.0f;
                var blue = currentPixel.Blue / 255.0f;

                var maximum = Math.Max(Math.Max(red, green), blue);
                var minimum = Math.Min(Math.Min(red, green), blue);

                var difference = (maximum - minimum) / 2;
                var summary = minimum + maximum;

                float hue;
                float saturation;
                var luminance = difference / 2;

                if (difference == 0)
                {
                    hue = 0;
                    saturation = 0;
                }
                else
                {
                    saturation = luminance <= 0.5 ? difference / summary : difference / (2 - summary);

                    if (Math.Abs(red - maximum) < 0.0001)
                    {
                        hue = (green - blue) / (6 * difference);
                    }
                    else if (Math.Abs(green - maximum) < 0.0001)
                    {
                        hue = 1.0f / 3 + (blue - red) / (6 * difference);
                    }
                    else
                    {
                        hue = 1.0f / 3 + (red - green) / (6 * difference);
                    }
                }

                if (hue < 0)
                {
                    hue += 1;
                }

                if (hue > 1)
                {
                    hue -= 1;
                }

                hue *= 360;
                saturation *= 100;
                luminance *= 100;

                convertedBitmap
                    .SetPixel(x, y, new SKColor((byte) (hue * 255 / 360), (byte) saturation, (byte) luminance));
            }
        }

        return convertedBitmap;
    }

    public SKBitmap ToRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap();

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var currentPixel = bitmap.GetPixel(x, y);

                var hue = currentPixel.Red / 255f;
                var saturation = currentPixel.Green / 100f;
                var luminance = currentPixel.Blue / 100f;

                var red = luminance;
                var green = luminance;
                var blue = luminance;

                if (Math.Abs(saturation) > 0.0001)
                {
                    var correctedHue = hue / 360;

                    var secondIntermediateValue = luminance < 0.5
                        ? luminance * (1 + saturation)
                        : luminance + saturation - luminance * saturation;
                    var firstIntermediateValue = 2 * luminance - secondIntermediateValue;

                    red = ConvertHueToRgb(firstIntermediateValue, secondIntermediateValue, correctedHue + 1f / 3);
                    green = ConvertHueToRgb(firstIntermediateValue, secondIntermediateValue, correctedHue);
                    blue = ConvertHueToRgb(firstIntermediateValue, secondIntermediateValue, correctedHue - 1f / 3);
                }

                convertedBitmap.SetPixel(x, y,
                    new SKColor((byte) (red * byte.MaxValue), (byte) (green * byte.MaxValue),
                        (byte) (blue * byte.MaxValue)));
            }
        }

        return convertedBitmap;
    }

    private float ConvertHueToRgb(float firstValue, float secondValue, float correctedHue)
    {
        if (correctedHue < 0)
        {
            correctedHue += 1;
        }

        if (correctedHue > 1)
        {
            correctedHue -= 1;
        }

        if (2 * correctedHue < 1)
        {
            return secondValue;
        }

        if (3 * correctedHue < 2)
        {
            return (int) (firstValue + (secondValue - firstValue) * 6 * correctedHue);
        }

        if (6 * correctedHue < 1)
        {
            return (int) (firstValue + (secondValue - firstValue) * 6 * (2.0 / 3 - correctedHue));
        }

        return firstValue;
    }
}