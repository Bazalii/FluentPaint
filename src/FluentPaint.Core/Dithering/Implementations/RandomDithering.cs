using System.Drawing;
using FluentPaint.Core.Pictures;
using SkiaSharp;

namespace FluentPaint.Core.Dithering.Implementations;

public class RandomDithering : IDithering
{
    private readonly Random _random = new();

    public FluentBitmap Dithering(FluentBitmap bitmap, int bitDepth)
    {
        var resultBitmap = new FluentBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var randomRed = _random.Next(GetLeftColor(pixel.Red, bitDepth), GetRightColor(pixel.Red, bitDepth));
                var randomGreen = _random.Next(GetLeftColor(pixel.Green, bitDepth), GetRightColor(pixel.Green, bitDepth));
                var randomBlue = _random.Next(GetLeftColor(pixel.Blue, bitDepth), GetRightColor(pixel.Blue, bitDepth));

                var red = CalculateNewPixelComponent(randomRed, pixel.Red, bitDepth);
                var green = CalculateNewPixelComponent(randomGreen, pixel.Green, bitDepth);
                var blue = CalculateNewPixelComponent(randomBlue, pixel.Blue, bitDepth);

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

    private byte CalculateNewPixelComponent(int randomValue, byte pixelComponent, int bitDepth)
    {
        return randomValue > pixelComponent
            ? GetLeftColor(pixelComponent, bitDepth)
            : GetRightColor(pixelComponent, bitDepth);
    }
}