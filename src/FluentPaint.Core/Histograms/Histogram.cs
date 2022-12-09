using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Histograms;

public class Histogram
{
    private List<Coordinates>[]? _redHistogram;
    private List<Coordinates>[]? _greenHistogram;
    private List<Coordinates>[]? _blueHistogram;

    private readonly SKBitmap _bitmap;

    public Histogram(ColorChannels channels, SKBitmap bitmap)
    {
        _bitmap = bitmap;

        switch (channels)
        {
            case ColorChannels.First:
                _redHistogram = new List<Coordinates>[256];
                break;
            case ColorChannels.Second:
                _greenHistogram = new List<Coordinates>[256];
                break;
            case ColorChannels.Third:
                _blueHistogram = new List<Coordinates>[256];
                break;
            case ColorChannels.FirstAndSecond:
                _redHistogram = new List<Coordinates>[256];
                _greenHistogram = new List<Coordinates>[256];
                break;
            case ColorChannels.FirstAndThird:
                _redHistogram = new List<Coordinates>[256];
                _blueHistogram = new List<Coordinates>[256];
                break;
            case ColorChannels.SecondAndThird:
                _greenHistogram = new List<Coordinates>[256];
                _blueHistogram = new List<Coordinates>[256];
                break;
            case ColorChannels.All:
                _redHistogram = new List<Coordinates>[256];
                _greenHistogram = new List<Coordinates>[256];
                _blueHistogram = new List<Coordinates>[256];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(channels), channels, null);
        }

        for (var i = 0; i < 256; i++)
        {
            if (_redHistogram is not null)
            {
                _redHistogram[i] = new List<Coordinates>();
            }
            if (_greenHistogram is not null)
            {
                _greenHistogram[i] = new List<Coordinates>();
            }
            if (_blueHistogram is not null)
            {
                _blueHistogram[i] = new List<Coordinates>();
            }
        }
    }

    public List<List<int>> CreateHistograms(double ignoredPercent)
    {
        var pixelsValue = CreatePixelsValue();
        List<Coordinates> ignoredPixels = new();

        if (ignoredPercent != 0)
        {
            ignoredPixels = GetIgnorePixel(pixelsValue, ignoredPercent);
        }

        for (var y = 0; y < _bitmap.Height; y++)
        {
            for (var x = 0; x < _bitmap.Width; x++)
            {
                var pixel = _bitmap.GetPixel(x, y);

                if (ignoredPixels.Any(coordinate => coordinate.X == x && coordinate.Y == y)) continue;

                _redHistogram?[pixel.Red].Add(new Coordinates(x, y));
                _greenHistogram?[pixel.Green].Add(new Coordinates(x, y));
                _blueHistogram?[pixel.Blue].Add(new Coordinates(x, y));
            }
        }

        var resultingHistograms = new List<List<int>>();

        if (_redHistogram is not null)
        {
            resultingHistograms.Add(_redHistogram.Select(list => list.Count).ToList());
        }

        if (_greenHistogram is not null)
        {
            resultingHistograms.Add(_greenHistogram.Select(list => list.Count).ToList());
        }

        if (_blueHistogram is not null)
        {
            resultingHistograms.Add(_blueHistogram.Select(list => list.Count).ToList());
        }

        return resultingHistograms;
    }

    public SKBitmap Correct()
    {
        var redHistogram = _redHistogram is null ? null : new List<Coordinates>[256];
        var greenHistogram = _greenHistogram is null ? null : new List<Coordinates>[256];
        var blueHistogram = _blueHistogram is null ? null : new List<Coordinates>[256];

        var min = GetMinValue();
        var max = GetMaxValue();

        var oldIndex = min;
        var newIndex = 0;
        var unusedCount = min + 255 - max;
        double correction = 0;

        while (newIndex < 256)
        {
            correction += (double) (256 - unusedCount) / (unusedCount + 1);

            while (correction >= 1)
            {
                if (_redHistogram is not null)
                {
                    redHistogram![newIndex] = _redHistogram[oldIndex];
                }

                if (_greenHistogram is not null)
                {
                    greenHistogram![newIndex] = _greenHistogram[oldIndex];
                }

                if (_blueHistogram is not null)
                {
                    blueHistogram![newIndex] = _blueHistogram[oldIndex];
                }

                oldIndex = Increment(oldIndex);
                newIndex = Increment(newIndex);
                correction -= 1;
            }

            if (newIndex == 256) break;

            if (redHistogram is not null)
            {
                redHistogram[newIndex] = new List<Coordinates>();
            }

            if (greenHistogram is not null)
            {
                greenHistogram[newIndex] = new List<Coordinates>();
            }

            if (blueHistogram is not null)
            {
                blueHistogram[newIndex] = new List<Coordinates>();
            }

            newIndex = Increment(newIndex);
        }

        for (var i = 0; i < 256; i++)
        {
            if (redHistogram is not null)
            {
                for (var j = 0; j < redHistogram[i].Count; j++)
                {
                    var x = redHistogram[i][j].X;
                    var y = redHistogram[i][j].Y;
                    var pixel = _bitmap.GetPixel(x, y);

                    _bitmap.SetPixel(x, y, new SKColor((byte) i, pixel.Green, pixel.Blue));
                }
            }

            if (greenHistogram is not null)
            {
                for (var j = 0; j < greenHistogram[i].Count; j++)
                {
                    var x = greenHistogram[i][j].X;
                    var y = greenHistogram[i][j].Y;
                    var pixel = _bitmap.GetPixel(x, y);

                    _bitmap.SetPixel(x, y, new SKColor(pixel.Red, (byte) i, pixel.Blue));
                }
            }

            if (blueHistogram is not null)
            {
                for (var j = 0; j < blueHistogram[i].Count; j++)
                {
                    var x = blueHistogram[i][j].X;
                    var y = blueHistogram[i][j].Y;
                    var pixel = _bitmap.GetPixel(x, y);

                    _bitmap.SetPixel(x, y, new SKColor(pixel.Red, pixel.Green, (byte) i));
                }
            }
        }

        _redHistogram = redHistogram;
        _greenHistogram = greenHistogram;
        _blueHistogram = blueHistogram;

        return _bitmap;
    }

    private List<Coordinates>[] CreatePixelsValue()
    {
        var pixels = new List<Coordinates>[766];

        for (var i = 0; i < 766; i++)
        {
            pixels[i] = new List<Coordinates>();
        }

        for (var y = 0; y < _bitmap.Height; y++)
        {
            for (var x = 0; x < _bitmap.Width; x++)
            {
                var pixel = _bitmap.GetPixel(x, y);

                pixels[pixel.Red + pixel.Green + pixel.Blue].Add(new Coordinates(x, y));
            }
        }

        return pixels;
    }

    private List<Coordinates> GetIgnorePixel(IReadOnlyList<List<Coordinates>> pixelsValue, double ignorePercent)
    {
        var ignoredPixels = new List<Coordinates>();
        var ignoredPixelsAmount = (int) (_bitmap.Height * _bitmap.Width * ignorePercent);

        for (var i = 0; i < 766; i++)
        {
            for (var j = 0; j < pixelsValue[i].Count; j++)
            {
                if (ignoredPixelsAmount == 0) break;

                ignoredPixels.Add(pixelsValue[i][j]);
                ignoredPixelsAmount--;
            }

            if (ignoredPixelsAmount == 0) break;
        }

        ignoredPixelsAmount = (int) (_bitmap.Height * _bitmap.Width * ignorePercent);

        for (var i = 765; i >= 0; i--)
        {
            for (var j = 0; j < pixelsValue[i].Count; j++)
            {
                if (ignoredPixelsAmount == 0) break;

                ignoredPixels.Add(pixelsValue[i][j]);
                ignoredPixelsAmount--;
            }

            if (ignoredPixelsAmount == 0) break;
        }

        return ignoredPixels;
    }

    private int GetMinValue()
    {
        for (var i = 0; i < 256; i++)
        {
            if ((_redHistogram is not null && _redHistogram[i].Count != 0) ||
                (_greenHistogram is not null && _greenHistogram[i].Count != 0) ||
                (_blueHistogram is not null && _blueHistogram[i].Count != 0))
            {
                return i;
            }
        }

        throw new Exception("Data Error");
    }

    private int GetMaxValue()
    {
        for (var i = 255; i >= 0; i--)
        {
            if ((_redHistogram is not null && _redHistogram[i].Count != 0) ||
                (_greenHistogram is not null && _greenHistogram[i].Count != 0) ||
                (_blueHistogram is not null && _blueHistogram[i].Count != 0))
            {
                return i;
            }
        }

        throw new Exception("Data Error");
    }

    private int Increment(int index)
    {
        if (_redHistogram is not null || _greenHistogram is not null || _blueHistogram is not null)
        {
            return index + 1;
        }

        return index;
    }
}