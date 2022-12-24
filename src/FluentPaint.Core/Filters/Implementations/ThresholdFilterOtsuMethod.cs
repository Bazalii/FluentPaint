using FluentPaint.Core.Enums;
using FluentPaint.Core.Histograms;
using FluentPaint.Core.Pictures;
using SkiaSharp;

namespace FluentPaint.Core.Filters.Implementations;

public class ThresholdFilterOtsuMethod : IFilter
{
    private byte? _redThreshold;
    private byte? _greenThreshold;
    private byte? _blueThreshold;

    public FluentBitmap Filter(ColorChannels channels, FluentBitmap bitmap)
    {
        var resultBitmap = new FluentBitmap(bitmap.Width, bitmap.Height);

        var histogram = new Histogram(channels, bitmap);
        var histograms = histogram.CreateHistograms(0);

        FindAllThresholds(channels, histograms, bitmap.Height * bitmap.Width);

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
                    red = pixel.Red < _redThreshold ? (byte) 0 : (byte) 255;
                }

                if (channels is ColorChannels.All or ColorChannels.Second or ColorChannels.FirstAndSecond
                    or ColorChannels.SecondAndThird)
                {
                    green = pixel.Green < _greenThreshold ? (byte) 0 : (byte) 255;
                }

                if (channels is ColorChannels.All or ColorChannels.Third or ColorChannels.FirstAndThird
                    or ColorChannels.SecondAndThird)
                {
                    blue = pixel.Blue < _blueThreshold ? (byte) 0 : (byte) 255;
                }

                resultBitmap.SetPixel(x, y, new SKColor(red, green, blue));
            }
        }

        return resultBitmap;
    }

    private void FindAllThresholds(ColorChannels channels, List<List<int>> histograms, long size)
    {
        switch (channels)
        {
            case ColorChannels.First:
                _redThreshold = CalculateThreshold(histograms[0], size);
                break;
            case ColorChannels.Second:
                _greenThreshold = CalculateThreshold(histograms[0], size);
                break;
            case ColorChannels.Third:
                _blueThreshold = CalculateThreshold(histograms[0], size);
                break;
            case ColorChannels.FirstAndSecond:
                _redThreshold = CalculateThreshold(histograms[0], size);
                _greenThreshold = CalculateThreshold(histograms[1], size);
                break;
            case ColorChannels.FirstAndThird:
                _redThreshold = CalculateThreshold(histograms[0], size);
                _blueThreshold = CalculateThreshold(histograms[1], size);
                break;
            case ColorChannels.SecondAndThird:
                _greenThreshold = CalculateThreshold(histograms[0], size);
                _blueThreshold = CalculateThreshold(histograms[1], size);
                break;
            case ColorChannels.All:
                _redThreshold = CalculateThreshold(histograms[0], size);
                _greenThreshold = CalculateThreshold(histograms[1], size);
                _blueThreshold = CalculateThreshold(histograms[2], size);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(channels), channels, null);
        }
    }

    private byte CalculateThreshold(List<int> histogram, long size)
    {
        ulong maxIntensitySum = 0;
        for (var i = 0; i < 256; ++i)
        {
            maxIntensitySum += (ulong) (i * histogram[i]);
        }

        var maxSigma = -1.0f;
        var threshold = 0;

        ulong intensitySum = 0;
        long sumPixels = 0;

        for (var i = 0; i < 256; ++i)
        {
            sumPixels += histogram[i];

            if (sumPixels == 0) continue;
            if (sumPixels == size) break;

            intensitySum += (ulong) (i * histogram[i]);

            var probability = (float) sumPixels / size;

            var mean = (float) intensitySum / sumPixels - (float) (maxIntensitySum - intensitySum) / (size - sumPixels);

            var sigma = probability * (1 - probability) * mean * mean;

            if (!(sigma > maxSigma)) continue;

            maxSigma = sigma;
            threshold = i;
        }

        return (byte) threshold;
    }
}