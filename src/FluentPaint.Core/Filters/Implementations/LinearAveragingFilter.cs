using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Filters.Implementations;

public class LinearAveragingFilter : IFilter
{
    private readonly int _radius;

    public LinearAveragingFilter(int radius)
    {
        _radius = radius;
    }

    public SKBitmap Filter(ColorChannels channels, SKBitmap bitmap)
    {
        var resultBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var samplingPixels = GetSamplingPixels(bitmap, x, y);

                byte red = 0;
                byte green = 0;
                byte blue = 0;

                if (channels is ColorChannels.All or ColorChannels.First or ColorChannels.FirstAndSecond
                    or ColorChannels.FirstAndThird)
                {
                    red = (byte)(samplingPixels.Select(pixel => (int)pixel.Red).Sum() / samplingPixels.Count);
                }

                if (channels is ColorChannels.All or ColorChannels.Second or ColorChannels.FirstAndSecond
                    or ColorChannels.SecondAndThird)
                {
                    green = (byte)(samplingPixels.Select(pixel => (int)pixel.Green).Sum() / samplingPixels.Count);
                }

                if (channels is ColorChannels.All or ColorChannels.Third or ColorChannels.FirstAndThird
                    or ColorChannels.SecondAndThird)
                {
                    blue = (byte)(samplingPixels.Select(pixel => (int)pixel.Blue).Sum() / samplingPixels.Count);
                }

                resultBitmap.SetPixel(x, y, new SKColor(red, green, blue));
            }
        }

        return resultBitmap;
    }

    private IList<SKColor> GetSamplingPixels(SKBitmap bitmap, int x, int y)
    {
        var result = new List<SKColor>();

        for (var i = x - _radius; i <= x + _radius; i++)
        {
            for (var j = y - _radius; j <= y + _radius; j++)
            {
                if (i < 0 || j < 0 || i > bitmap.Width || j > bitmap.Height) continue;

                result.Add(bitmap.GetPixel(i, j));
            }
        }

        return result;
    }
}