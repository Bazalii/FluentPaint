using SkiaSharp;

namespace FluentPaint.Core.Dithering.Implementations;

public class OrderedDithering : IDithering
{
    private double[,] matrix = new double[8, 8]
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

    public SKBitmap Dithering(SKBitmap bitmap, int bitDepth)
    {
        var resultBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
        
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                
                var red = matrix[x % 7, y % 7] / 64 * 255 > pixel.Red
                    ? GetLeftColor(pixel.Red, bitDepth)
                    : GetRightColor(pixel.Red, bitDepth);
                var green = matrix[x % 7, y % 7] / 64 * 255 > pixel.Green
                    ? GetLeftColor(pixel.Green, bitDepth)
                    : GetRightColor(pixel.Green, bitDepth);
                var blue = matrix[x % 7, y % 7] / 64 * 255 > pixel.Blue
                    ? GetLeftColor(pixel.Blue, bitDepth)
                    : GetRightColor(pixel.Blue, bitDepth);
                
                resultBitmap.SetPixel(x, y, new SKColor(red, green, blue));
            }
        }

        return resultBitmap;
    }
    
    private byte GetLeftColor(byte pixelColor, int bitDepth)
    {
        double color = 0;

        while (color <= 255)
        {
            if (pixelColor <= color + 255 / (Math.Pow(2, bitDepth) - 1))
                break;
            color += 255 / (Math.Pow(2, bitDepth) - 1);
        }

        return (byte)color;
    }

    private byte GetRightColor(byte pixelColor, int bitDepth)
    {
        double color = 0;

        while (color <= 255)
        {
            if (pixelColor <= color + 255 / (Math.Pow(2, bitDepth) - 1))
                break;
            color += 255 / (Math.Pow(2, bitDepth) - 1);
        }

        return (byte)(color + 255 / (Math.Pow(2, bitDepth) - 1));
    }
}