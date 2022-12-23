using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Filters.Implementations;

public class GaussianFilter : IFilter
{
    private readonly float _sigma;

    public GaussianFilter(float sigma)
    {
        _sigma = sigma;
    }

    public SKBitmap Filter(ColorChannels channels, SKBitmap bitmap)
    {
        var resultBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        var gaussian = CreateGaussian();
        var radius = (int)(3 * _sigma);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var redValues = new List<double>();
                var greenValues = new List<double>();
                var blueValues = new List<double>();

                for (var i = -radius; i <= radius; i++)
                {
                    for (var j = -radius; j <= radius; j++)
                    {
                        if (-1 < j + y && j + y < bitmap.Height && -1 < i + x && i + x < bitmap.Width)
                        {
                            redValues.Add(bitmap.GetPixel(i + x, j + y).Red * gaussian[i + radius, j + radius]);
                            greenValues.Add(bitmap.GetPixel(i + x, j + y).Green * gaussian[i + radius, j + radius]);
                            blueValues.Add(bitmap.GetPixel(i + x, j + y).Blue * gaussian[i + radius, j + radius]);
                        }
                        else
                        {
                            redValues.Add(
                                bitmap.GetPixel(i + x < 0 ? 0 : i + x > bitmap.Width - 1 ? bitmap.Width - 1 : i + x,
                                    j + y < 0 ? 0 : j + y > bitmap.Height - 1 ? bitmap.Height - 1 : j + y).Red *
                                gaussian[i + radius, j + radius]);
                            greenValues.Add(
                                bitmap.GetPixel(i + x < 0 ? 0 : i + x > bitmap.Width - 1 ? bitmap.Width - 1 : i + x,
                                    j + y < 0 ? 0 : j + y > bitmap.Height - 1 ? bitmap.Height - 1 : j + y).Green *
                                gaussian[i + radius, j + radius]);
                            blueValues.Add(
                                bitmap.GetPixel(i + x < 0 ? 0 : i + x > bitmap.Width - 1 ? bitmap.Width - 1 : i + x,
                                    j + y < 0 ? 0 : j + y > bitmap.Height - 1 ? bitmap.Height - 1 : j + y).Blue *
                                gaussian[i + radius, j + radius]);
                        }
                    }
                }

                resultBitmap.SetPixel(x, y,
                    new SKColor((byte)redValues.Sum(), (byte)greenValues.Sum(), (byte)blueValues.Sum()));
            }
        }

        return resultBitmap;
    }

    private double[,] CreateGaussian()
    {
        var sum = 0.0;
        var radius = 3 * _sigma;
        var gaussian = new double[(int)(2 * radius) + 1, (int)(2 * radius) + 1];

        for (var i = 0; i <= (int)(2 * radius); i++)
        {
            for (var j = 0; j <= (int)(2 * radius); j++)
            {
                gaussian[i, j] =
                    Math.Exp(-(Math.Pow(i - radius, 2) + Math.Pow(j - radius, 2)) / (2 * _sigma * _sigma)) /
                    (2 * Math.PI * _sigma * _sigma);
                sum += gaussian[i, j];
            }
        }

        for (var i = 0; i <= 3 * _sigma; i++)
        {
            for (var j = 0; j <= 3 * _sigma; j++)
            {
                gaussian[i, j] /= sum;
            }
        }

        return gaussian;
    }
}