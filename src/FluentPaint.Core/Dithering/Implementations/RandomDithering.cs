using System.Drawing;
using SkiaSharp;

namespace FluentPaint.Core.Dithering.Implementations;

public class RandomDithering : IDithering
{
    private readonly Random _random = new();

    public SKBitmap Dithering(SKBitmap bitmap, int bitDepth)
    {
        var resultBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var randomValue = 0 + _random.Next(0, 255);

                var red = CalculateNewPixelComponent(randomValue, pixel.Red, bitDepth);
                var green = CalculateNewPixelComponent(randomValue, pixel.Green, bitDepth);
                var blue = CalculateNewPixelComponent(randomValue, pixel.Blue, bitDepth);

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

        return (byte) color;
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

        return (byte) (color + 255 / (Math.Pow(2, bitDepth) - 1));
    }

    private byte CalculateNewPixelComponent(int randomValue, byte pixelComponent, int bitDepth)
    {
        return randomValue > pixelComponent
            ? GetLeftColor(pixelComponent, bitDepth)
            : GetRightColor(pixelComponent, bitDepth);
    }
}