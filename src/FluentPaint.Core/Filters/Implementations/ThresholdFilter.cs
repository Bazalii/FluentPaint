using FluentPaint.Core.Enums;
using FluentPaint.Core.Pictures;
using SkiaSharp;

namespace FluentPaint.Core.Filters.Implementations;

public class ThresholdFilter : IFilter
{
    private readonly byte _threshold;

    public ThresholdFilter(byte threshold)
    {
        _threshold = threshold;
    }

    public FluentBitmap Filter(ColorChannels channels, FluentBitmap bitmap)
    {
        var resultBitmap = new FluentBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                byte red = 0;
                byte green = 0;
                byte blue = 0;

                if (channels is ColorChannels.All or ColorChannels.First or ColorChannels.FirstAndSecond
                    or ColorChannels.FirstAndThird)
                {
                    red = pixel.Red < _threshold ? (byte) 0 : (byte) 255;
                }

                if (channels is ColorChannels.All or ColorChannels.Second or ColorChannels.FirstAndSecond
                    or ColorChannels.SecondAndThird)
                {
                    green = pixel.Green < _threshold ? (byte) 0 : (byte) 255;
                }

                if (channels is ColorChannels.All or ColorChannels.Third or ColorChannels.FirstAndThird
                    or ColorChannels.SecondAndThird)
                {
                    blue = pixel.Blue < _threshold ? (byte) 0 : (byte) 255;
                }

                resultBitmap.SetPixel(x, y, new SKColor(red, green, blue));
            }
        }

        return resultBitmap;
    }
}