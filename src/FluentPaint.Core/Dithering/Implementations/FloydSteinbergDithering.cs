using System.Drawing;
using SkiaSharp;

namespace FluentPaint.Core.Dithering.Implementations;

public class FloydSteinbergDithering : IDithering
{
    public SKBitmap Dithering(SKBitmap bitmap, int bitDepth)
    {
        var errorMatrix = new double[bitmap.Width, bitmap.Height, 3];

        var resultBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var red = GetNearestColor(pixel.Red + errorMatrix[x, y, 0], bitDepth);
                var green = GetNearestColor(pixel.Green + errorMatrix[x, y, 1], bitDepth);
                var blue = GetNearestColor(pixel.Blue + errorMatrix[x, y, 2], bitDepth);

                var redError = pixel.Red - red;
                var greenError = pixel.Green - green;
                var blueError = pixel.Blue - blue;

                if (x < bitmap.Width - 1)
                {
                    errorMatrix[x + 1, y, 0] += redError * 0.4375;
                    errorMatrix[x + 1, y, 1] += greenError * 0.4375;
                    errorMatrix[x + 1, y, 2] += blueError * 0.4375;
                }

                if (y < bitmap.Height - 1)
                {
                    if (x > 0)
                    {
                        errorMatrix[x - 1, y + 1, 0] += redError * 0.1875;
                        errorMatrix[x - 1, y + 1, 1] += greenError * 0.1875;
                        errorMatrix[x - 1, y + 1, 2] += blueError * 0.1875;
                    }

                    errorMatrix[x, y + 1, 0] += redError * 0.3125;
                    errorMatrix[x, y + 1, 1] += greenError * 0.3125;
                    errorMatrix[x, y + 1, 2] += blueError * 0.3125;

                    if (x < bitmap.Width - 1)
                    {
                        errorMatrix[x + 1, y + 1, 0] += redError * 0.0625;
                        errorMatrix[x + 1, y + 1, 1] += greenError * 0.0625;
                        errorMatrix[x + 1, y + 1, 2] += blueError * 0.0625;
                    }
                }

                resultBitmap.SetPixel(x, y, new SKColor(red, green, blue));
            }
        }

        return resultBitmap;
    }

    private byte GetNearestColor(double pixelColor, int bitDepth)
    {
        double color = 0;
        var prevDiv = pixelColor;

        while (color <= 255)
        {
            color += 255 / (Math.Pow(2, bitDepth) - 1);

            if ((prevDiv < Math.Abs(color - pixelColor)))
            {
                color -= 255 / (Math.Pow(2, bitDepth) - 1);
                break;
            }

            prevDiv = Math.Abs(color - pixelColor);
        }

        return (byte) color;
    }
}