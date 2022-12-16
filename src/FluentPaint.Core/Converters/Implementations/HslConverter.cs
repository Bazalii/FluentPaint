using FluentPaint.Core.Pictures;
using SkiaSharp;

namespace FluentPaint.Core.Converters.Implementations;

/// <summary>
/// Provides methods to convert pictures from RGB to HSL and in the opposite direction.
/// </summary>
public class HslConverter : IConverter
{
    /// <summary>
    /// Converts picture from RGB to HSL.
    /// </summary>
    /// <param name="bitmap"> <see cref="FluentBitmap"/> that contains all pixels of the picture. </param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    public FluentBitmap FromRgb(FluentBitmap bitmap)
    {
        var convertedBitmap = new FluentBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var currentPixel = bitmap.GetPixel(x, y);

                var red = currentPixel.Red / 255f;
                var green = currentPixel.Green / 255f;
                var blue = currentPixel.Blue / 255f;

                var maximum = Math.Max(Math.Max(red, green), blue);
                var minimum = Math.Min(Math.Min(red, green), blue);

                var difference = maximum - minimum;
                var summary = minimum + maximum;

                var hue = 0.0f;
                var saturation = 0.0f;
                var luminance = summary / 2;

                if (Math.Abs(difference) > 0.0001)
                {
                    saturation = luminance <= 0.5 ? difference / summary : difference / (2f - summary);

                    var firstIntermediateValue = ((maximum - red) / 6f + difference / 2f) / difference;
                    var secondIntermediateValue = ((maximum - green) / 6f + difference / 2f) / difference;
                    var thirdIntermediateValue = ((maximum - blue) / 6f + difference / 2f) / difference;

                    hue = Math.Abs(red - maximum) >= 0.0001
                        ? Math.Abs(green - maximum) >= 0.0001
                            ? 2f / 3 + secondIntermediateValue - firstIntermediateValue
                            : 1f / 3 + firstIntermediateValue - thirdIntermediateValue
                        : thirdIntermediateValue - secondIntermediateValue;

                    if (hue < 0)
                    {
                        hue += 1;
                    }

                    if (hue > 1)
                    {
                        hue -= 1;
                    }
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

    /// <summary>
    /// Converts picture from HSL to RGB.
    /// </summary>
    /// <param name="bitmap"> <see cref="FluentBitmap"/> that contains all pixels of the picture. </param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    public FluentBitmap ToRgb(FluentBitmap bitmap)
    {
        var convertedBitmap = new FluentBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var currentPixel = bitmap.GetPixel(x, y);

                var hue = currentPixel.Red / 255f * 360;
                var saturation = currentPixel.Green / 100f;
                var luminance = currentPixel.Blue / 100f;

                var red = luminance;
                var green = luminance;
                var blue = luminance;

                if (Math.Abs(saturation) > 0.0001)
                {
                    hue /= 360;

                    var secondIntermediateValue = luminance < 0.5
                        ? luminance * (1f + saturation)
                        : luminance + saturation - luminance * saturation;
                    var firstIntermediateValue = 2f * luminance - secondIntermediateValue;

                    red = ConvertHueToRgb(firstIntermediateValue, secondIntermediateValue, hue + 1f / 3);
                    green = ConvertHueToRgb(firstIntermediateValue, secondIntermediateValue, hue);
                    blue = ConvertHueToRgb(firstIntermediateValue, secondIntermediateValue, hue - 1f / 3);
                }

                convertedBitmap.SetPixel(x, y,
                    new SKColor(
                        (byte) (red * byte.MaxValue),
                        (byte) (green * byte.MaxValue),
                        (byte) (blue * byte.MaxValue)
                    ));
            }
        }

        return convertedBitmap;
    }

    /// <summary>
    /// Helper method to convert hue to RGB.
    /// </summary>
    /// <param name="firstValue"> Received intermediate value. </param>
    /// <param name="secondValue"> Received intermediate value. </param>
    /// <param name="hue"> Received hue. </param>
    /// <returns>
    /// Converted hue.
    /// </returns>
    private float ConvertHueToRgb(float firstValue, float secondValue, float hue)
    {
        if (hue < 0)
        {
            hue += 1;
        }

        if (hue > 1)
        {
            hue -= 1;
        }

        if (6.0 * hue < 1)
        {
            return firstValue + (secondValue - firstValue) * 6f * hue;
        }

        if (2.0 * hue < 1)
        {
            return secondValue;
        }

        if (3.0 * hue < 2)
        {
            return firstValue + (secondValue - firstValue) * 6f * (2.0f / 3 - hue);
        }

        return firstValue;
    }
}