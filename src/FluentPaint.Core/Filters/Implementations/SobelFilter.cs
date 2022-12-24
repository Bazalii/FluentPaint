using FluentPaint.Core.Enums;
using FluentPaint.Core.Pictures;
using SkiaSharp;

namespace FluentPaint.Core.Filters.Implementations;

public class SobelFilter : IFilter
{
    private readonly int[,] _gx =
    {
        { -1, 0, 1 },
        { -2, 0, 2 },
        { -1, 0, 1 }
    };

    private readonly int[,] _gy =
    {
        { 1, 2, 1 },
        { 0, 0, 0 },
        { -1, -2, -1 }
    };

    private readonly int _limit;

    public SobelFilter(int limit)
    {
        _limit = limit;
    }

    public FluentBitmap Filter(ColorChannels channels, FluentBitmap bitmap)
    {
        var resultBitmap = new FluentBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var redX = 0;
                var redY = 0;
                var greenX = 0;
                var greenY = 0;
                var blueX = 0;
                var blueY = 0;

                for (var i = -1; i < 2; i++)
                {
                    for (var j = -1; j < 2; j++)
                    {
                        if (x + i < 0 || y + j < 0 || x + i > bitmap.Width || y + j > bitmap.Height) continue;

                        var pixel = bitmap.GetPixel(x + i, y + j);

                        redX += _gx[i + 1, j + 1] * pixel.Red;
                        redY += _gy[i + 1, j + 1] * pixel.Red;

                        greenX += _gx[i + 1, j + 1] * pixel.Green;
                        greenY += _gy[i + 1, j + 1] * pixel.Green;

                        blueX += _gx[i + 1, j + 1] * pixel.Blue;
                        blueY += _gy[i + 1, j + 1] * pixel.Blue;
                    }
                }

                byte red = 0;
                byte green = 0;
                byte blue = 0;

                if (channels is ColorChannels.All or ColorChannels.First or ColorChannels.FirstAndSecond
                    or ColorChannels.FirstAndThird)
                {
                    red = GetColor(redX, redY);
                }

                if (channels is ColorChannels.All or ColorChannels.Second or ColorChannels.FirstAndSecond
                    or ColorChannels.SecondAndThird)
                {
                    blue = GetColor(blueX, blueY);
                }

                if (channels is ColorChannels.All or ColorChannels.Third or ColorChannels.FirstAndThird
                    or ColorChannels.SecondAndThird)
                {
                    green = GetColor(greenX, greenY);
                }

                resultBitmap.SetPixel(x, y, new SKColor(red, green, blue));
            }
        }

        return resultBitmap;
    }

    private byte GetColor(int colorX, int colorY)
    {
        var color = 255 - Math.Abs(colorX) - Math.Abs(colorY);

        if (colorX * colorX + colorY * colorY > _limit * _limit)
        {
            color = 0;
        }

        return (byte) color;
    }
}