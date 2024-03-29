using FluentPaint.Core.Pictures;
using SkiaSharp;

namespace FluentPaint.Core.Dithering.Implementations;

public class OrderedDithering : IDithering
{
    private readonly double[,] _matrix =
    {
        { 0, 32, 8, 40, 2, 34, 10, 42 },
        { 48, 16, 56, 24, 50, 18, 58, 26 },
        { 12, 44, 4, 36, 14, 46, 6, 38 },
        { 60, 28, 52, 20, 62, 30, 54, 22 },
        { 3, 35, 11, 43, 1, 33, 9, 41 },
        { 51, 19, 59, 27, 49, 17, 57, 25 },
        { 15, 47, 7, 39, 13, 45, 5, 37 },
        { 63, 31, 55, 23, 61, 29, 53, 21 }
    };

    public FluentBitmap Dithering(FluentBitmap bitmap, int bitDepth)
    {
        var resultBitmap = new FluentBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var red = CalculateNewPixelComponent(_matrix[x % 8, y % 8], pixel.Red, bitDepth);
                var green = CalculateNewPixelComponent(_matrix[x % 8, y % 8], pixel.Green, bitDepth);
                var blue = CalculateNewPixelComponent(_matrix[x % 8, y % 8], pixel.Blue, bitDepth);

                resultBitmap.SetPixel(x, y, new SKColor(red, green, blue));
            }
        }

        return resultBitmap;
    }

    private byte GetLeftColor(byte pixelColor, int bitDepth)
    {
        double color = 0;

        while (color < 255 - 10e-5)
        {
            if (pixelColor < color + 255 / (Math.Pow(2, bitDepth) - 1 - 10e-5))
                break;
            color += 255 / (Math.Pow(2, bitDepth) - 1);
        }

        return (byte) color;
    }

    private byte GetRightColor(byte pixelColor, int bitDepth)
    {
        double color = 0;

        while (color < 255- 10e-5)
        {
            if (pixelColor > color + 255 / (Math.Pow(2, bitDepth) - 1))
            {
                color += 255 / (Math.Pow(2, bitDepth) - 1 - 10e-5);
                continue;
            }
            break;
        }

        return (byte) (color + 255 / (Math.Pow(2, bitDepth) - 1));
    }

    private byte CalculateNewPixelComponent(double matrixElement, byte pixelComponent, int bitDepth)
    {
        return GetLeftColor(pixelComponent, bitDepth) + matrixElement / 64 * 255 / (Math.Pow(2, bitDepth) - 1) > pixelComponent
            ? GetLeftColor(pixelComponent, bitDepth)
            : GetRightColor(pixelComponent, bitDepth);
    }
}