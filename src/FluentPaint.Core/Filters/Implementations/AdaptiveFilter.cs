using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Filters.Implementations;

public class AdaptiveFilter : IFilter
{
    private readonly float _sharpness;

    public AdaptiveFilter(float sharpness = 1.0f)
    {
        _sharpness = sharpness;
    }

    public SKBitmap Filter(ColorChannels channels, SKBitmap bitmap)
    {
        var resultBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var samplingPixels = new List<SKColor>();
                var min = new double[3];
                var max = new double[3];


                for (var j = y - 1; j < y + 2; j++)
                {
                    for (var i = x - 1; i < x + 2; i++)
                    {
                        if (-1 < j && j < bitmap.Height && -1 < i && i < bitmap.Width)
                        {
                            samplingPixels.Add(bitmap.GetPixel(i, j));
                        }
                        else
                        {
                            samplingPixels.Add(bitmap.GetPixel(
                                i < 0 ? 0 : i > bitmap.Width - 1 ? bitmap.Width - 1 : i,
                                j < 0 ? 0 : j > bitmap.Height - 1 ? bitmap.Height - 1 : j));
                        }
                    }
                }

                min[0] = Min(samplingPixels[7].Red / 255.0, samplingPixels[3].Red / 255.0,
                    samplingPixels[4].Red / 255.0, samplingPixels[5].Red / 255.0, samplingPixels[1].Red / 255.0);
                max[0] = Max(samplingPixels[7].Red / 255.0, samplingPixels[3].Red / 255.0,
                    samplingPixels[4].Red / 255.0, samplingPixels[5].Red / 255.0, samplingPixels[1].Red / 255.0);
                min[0] += Min(min[0], samplingPixels[0].Red / 255.0, samplingPixels[2].Red / 255.0,
                    samplingPixels[6].Red / 255.0, samplingPixels[8].Red / 255.0);
                max[0] += Max(max[0], samplingPixels[0].Red / 255.0, samplingPixels[2].Red / 255.0,
                    samplingPixels[6].Red / 255.0, samplingPixels[8].Red / 255.0);

                min[1] = Min(samplingPixels[7].Green / 255.0, samplingPixels[3].Green / 255.0,
                    samplingPixels[4].Green / 255.0, samplingPixels[5].Green / 255.0, samplingPixels[1].Green / 255.0);
                max[1] = Max(samplingPixels[7].Green / 255.0, samplingPixels[3].Green / 255.0,
                    samplingPixels[4].Green / 255.0, samplingPixels[5].Green / 255.0, samplingPixels[1].Green / 255.0);
                min[1] += Min(min[1], samplingPixels[0].Green / 255.0, samplingPixels[2].Green / 255.0,
                    samplingPixels[6].Green / 255.0, samplingPixels[8].Green / 255.0);
                max[1] += Max(max[1], samplingPixels[0].Green / 255.0, samplingPixels[2].Green / 255.0,
                    samplingPixels[6].Green / 255.0, samplingPixels[8].Green / 255.0);

                min[2] = Min(samplingPixels[7].Blue / 255.0, samplingPixels[3].Blue / 255.0,
                    samplingPixels[4].Blue / 255.0, samplingPixels[5].Blue / 255.0, samplingPixels[1].Blue / 255.0);
                max[2] = Max(samplingPixels[7].Blue / 255.0, samplingPixels[3].Blue / 255.0,
                    samplingPixels[4].Blue / 255.0, samplingPixels[5].Blue / 255.0, samplingPixels[1].Blue / 255.0);
                min[2] += Min(min[2], samplingPixels[0].Blue / 255.0, samplingPixels[2].Blue / 255.0,
                    samplingPixels[6].Blue / 255.0, samplingPixels[8].Blue / 255.0);
                max[2] += Max(max[2], samplingPixels[0].Blue / 255.0, samplingPixels[2].Blue / 255.0,
                    samplingPixels[6].Blue / 255.0, samplingPixels[8].Blue / 255.0);

                var value = new double[3];
                for (var i = 0; i < 3; i++)
                {
                    value[i] = Math.Min(Math.Max(Math.Min(2.0 - max[i], min[i]) / (max[i] + 1e-9), 0.0), 1.0) *
                               (-0.125 - 0.075 * _sharpness);
                }

                var weight = 4.0 * value[0] + 1.0;
                var red = value[0] * (
                    samplingPixels[7].Red / 255.0 + samplingPixels[3].Red / 255.0 + samplingPixels[5].Red / 255.0 +
                    samplingPixels[1].Red / 255.0) + samplingPixels[4].Red / 255.0;
                red = Math.Min(Math.Max(red / weight, 0.0), 1.0);

                weight = 4.0 * value[1] + 1.0;
                var green = value[1] * (
                    samplingPixels[7].Green / 255.0 + samplingPixels[3].Green / 255.0 +
                    samplingPixels[5].Green / 255.0 +
                    samplingPixels[1].Green / 255.0) + samplingPixels[4].Green / 255.0;
                green = Math.Min(Math.Max(green / weight, 0.0), 1.0);

                weight = 4.0 * value[2] + 1.0;
                var blue = value[2] * (
                    samplingPixels[7].Blue / 255.0 + samplingPixels[3].Blue / 255.0 + samplingPixels[5].Blue / 255.0 +
                    samplingPixels[1].Blue / 255.0) + samplingPixels[4].Blue / 255.0;
                blue = Math.Min(Math.Max(blue / weight, 0.0), 1.0);

                resultBitmap.SetPixel(x, y, new SKColor((byte)(red * 255), (byte)(green * 255), (byte)(blue * 255)));
            }
        }

        return resultBitmap;
    }

    private double Min(double val1, double val2, double val3, double val4, double val5)
    {
        var min = Math.Min(val1, val2);
        min = Math.Min(min, val3);
        min = Math.Min(min, val4);
        min = Math.Min(min, val5);

        return min;
    }

    private double Max(double val1, double val2, double val3, double val4, double val5)
    {
        var max = Math.Max(val1, val2);
        max = Math.Max(max, val3);
        max = Math.Max(max, val4);
        max = Math.Max(max, val5);

        return max;
    }
}