using SkiaSharp;

namespace FluentPaint.Core.Converters.Implementations;

/// <summary>
/// Provides methods to convert pictures from RGB to HSV, and in the opposite direction.
/// </summary>
public class HsvConverter : IConverter
{
    /// <summary>
    /// Convert picture from RGB to HSV.
    /// </summary>
    /// <param name="bitmap"><see cref="SKBitmap"/> Bitmap containing all pixels of the picture.</param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    public SKBitmap FromRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var currentPixel = bitmap.GetPixel(x, y);

                var red = currentPixel.Red;
                var green = currentPixel.Green;
                var blue = currentPixel.Blue;

                var hue = 0.0f;
                float saturation;

                var minimum = Math.Min(Math.Min(red, green), blue);
                float value = Math.Max(Math.Max(red, green), blue);
                var difference = value - minimum;

                if (value == 0)
                {
                    saturation = 0;
                }
                else
                {
                    saturation = difference / value;
                }

                if (saturation == 0)
                {
                    hue = 0;
                }
                else
                {
                    if (Math.Abs(red - value) < 0.0001)
                    {
                        hue = (green - blue) / difference;
                    }
                    else if (Math.Abs(green - value) < 0.0001)
                    {
                        hue = 2 + (blue - red) / difference;
                    }
                    else if (Math.Abs(blue - value) < 0.0001)
                    {
                        hue = 4 + (red - green) / difference;
                    }

                    hue *= 60;

                    if (hue < 0)
                    {
                        hue += 360;
                    }
                }

                saturation *= 100;
                value *= 100;

                convertedBitmap
                    .SetPixel(x, y, new SKColor((byte) (hue * 255 / 360), (byte) saturation, (byte) (value / 255)));
            }
        }

        return convertedBitmap;
    }

    /// <summary>
    /// Convert picture from HSV to RGB.
    /// </summary>
    /// <param name="bitmap"><see cref="SKBitmap"/> Bitmap containing all pixels of the picture.</param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    public SKBitmap ToRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var currentPixel = bitmap.GetPixel(x, y);

                var hue = currentPixel.Red / 255f;
                var saturation = currentPixel.Green / 100f;
                var value = currentPixel.Blue / 100f;

                var red = value;
                var green = value;
                var blue = value;

                if (Math.Abs(saturation) > 0.0001)
                {
                    hue *= 6;

                    if (Math.Abs(hue - 6f) < 1.0 / 1000.0)
                    {
                        hue = 0.0f;
                    }

                    var colorSpaceClass = (int) Math.Truncate(hue);

                    var firstIntermediateValue = hue - colorSpaceClass;
                    var secondIntermediateValue = value * (1.0f - saturation);
                    var thirdIntermediateValue = value * (1.0f - saturation * firstIntermediateValue);
                    var fourthIntermediateValue = value * (1.0f - saturation * (1.0f - firstIntermediateValue));

                    switch (colorSpaceClass)
                    {
                        case 0:
                            red = value;
                            green = fourthIntermediateValue;
                            blue = secondIntermediateValue;
                            break;
                        case 1:
                            red = thirdIntermediateValue;
                            green = value;
                            blue = secondIntermediateValue;
                            break;
                        case 2:
                            red = secondIntermediateValue;
                            green = value;
                            blue = fourthIntermediateValue;
                            break;
                        case 3:
                            red = secondIntermediateValue;
                            green = thirdIntermediateValue;
                            blue = value;
                            break;
                        case 4:
                            red = fourthIntermediateValue;
                            green = secondIntermediateValue;
                            blue = value;
                            break;
                        default:
                            red = value;
                            green = secondIntermediateValue;
                            blue = thirdIntermediateValue;
                            break;
                    }
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
}