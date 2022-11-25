using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Histograms;

public class Histogram
{
    private List<(int, int)>[]? _redHistogram;
    private List<(int, int)>[]? _greenHistogram;
    private List<(int, int)>[]? _blueHistogram;

    private SKBitmap _bitmap;

    public Histogram(ColorChannels channels, SKBitmap bitmap)
    {
        _bitmap = bitmap;

        switch (channels)
        {
            case ColorChannels.First:
                _redHistogram = new List<(int, int)>[256];
                break;
            case ColorChannels.Second:
                _greenHistogram = new List<(int, int)>[256];
                break;
            case ColorChannels.Third:
                _blueHistogram = new List<(int, int)>[256];
                break;
            case ColorChannels.FirstAndSecond:
                _redHistogram = new List<(int, int)>[256];
                _greenHistogram = new List<(int, int)>[256];
                break;
            case ColorChannels.FirstAndThird:
                _redHistogram = new List<(int, int)>[256];
                _blueHistogram = new List<(int, int)>[256];
                break;
            case ColorChannels.SecondAndThird:
                _greenHistogram = new List<(int, int)>[256];
                _blueHistogram = new List<(int, int)>[256];
                break;
            case ColorChannels.All:
                _redHistogram = new List<(int, int)>[256];
                _greenHistogram = new List<(int, int)>[256];
                _blueHistogram = new List<(int, int)>[256];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(channels), channels, null);
        }
        
        for (var i = 0; i < 256; i++)
        {
            if (_redHistogram != null)
                _redHistogram[i] = new List<(int, int)>();
            if (_greenHistogram != null)
                _greenHistogram[i] = new List<(int, int)>();
            if (_blueHistogram != null)
                _blueHistogram[i] = new List<(int, int)>();
        }
    }

    public void CreateHistograms(double ignorePercent)
    {
        var pixelsValue = CreatePixelsValue();
        List<(int, int)> ignorePixels = new();

        if (ignorePercent != 0)
            ignorePixels = GetIgnorePixel(pixelsValue, ignorePercent);

        for (var y = 0; y < _bitmap.Height; y++)
        {
            for (var x = 0; x < _bitmap.Width; x++)
            {
                var pixel = _bitmap.GetPixel(x, y);

                if (ignorePixels.Any(tuple => tuple == (x, y))) continue;
                
                _redHistogram?[pixel.Red].Add((x, y));
                _greenHistogram?[pixel.Green].Add((x, y));
                _blueHistogram?[pixel.Blue].Add((x, y));
            }
        }
    }

    public SKBitmap AutomaticCorrection()
    {
        var redHistogram = _redHistogram == null ? null : new List<(int, int)>[256];
        var greenHistogram = _greenHistogram == null ? null : new List<(int, int)>[256];
        var blueHistogram = _blueHistogram == null ? null : new List<(int, int)>[256];

        var min = GetMinValue();
        var max = GetMaxValue();

        var oldIndex = min;
        var newIndex = 0;
        var unusedCount = min + 255 - max;
        double correction = 0;

        while (newIndex < 256)
        {
            correction += (double)(256 - unusedCount) / (unusedCount + 1);

            while (correction >= 1)
            {
                if (_redHistogram != null)
                    redHistogram![newIndex] = _redHistogram[oldIndex];
                if (_greenHistogram != null)
                    greenHistogram![newIndex] = _greenHistogram[oldIndex];
                if (_blueHistogram != null)
                    blueHistogram![newIndex] = _blueHistogram[oldIndex];
                oldIndex = Increment(oldIndex);
                newIndex = Increment(newIndex);
                correction -= 1;
            }

            if (newIndex == 256)
                break;

            if (redHistogram != null)
                redHistogram[newIndex] = new List<(int, int)>();
            if (greenHistogram != null)
                greenHistogram[newIndex] = new List<(int, int)>();
            if (blueHistogram != null)
                blueHistogram[newIndex] = new List<(int, int)>();
            newIndex = Increment(newIndex);
        }

        for (var i = 0; i < 256; i++)
        {
            if (redHistogram != null)
                for (var j = 0; j < redHistogram[i].Count; j++)
                {
                    var x = redHistogram[i][j].Item1;
                    var y = redHistogram[i][j].Item2;
                    var pixel = _bitmap.GetPixel(x, y);
                    
                    _bitmap.SetPixel(x, y, new SKColor((byte)i, pixel.Green, pixel.Blue));
                }
            if (greenHistogram != null)
                for (var j = 0; j < greenHistogram[i].Count; j++)
                {
                    var x = greenHistogram[i][j].Item1;
                    var y = greenHistogram[i][j].Item2;
                    var pixel = _bitmap.GetPixel(x, y);
                    
                    _bitmap.SetPixel(x, y, new SKColor(pixel.Red, (byte)i, pixel.Blue));
                }
            if (blueHistogram != null)
                for (var j = 0; j < blueHistogram[i].Count; j++)
                {
                    var x = blueHistogram[i][j].Item1;
                    var y = blueHistogram[i][j].Item2;
                    var pixel = _bitmap.GetPixel(x, y);
                    
                    _bitmap.SetPixel(x, y, new SKColor(pixel.Red, pixel.Green, (byte)i));
                }
        }

        _redHistogram = redHistogram;
        _greenHistogram = greenHistogram;
        _blueHistogram = blueHistogram;

        return _bitmap;
    }

    private List<(int, int)>[] CreatePixelsValue()
    {
        var pixelsValue = new List<(int, int)>[766];

        for (var i = 0; i < 766; i++)
        {
            pixelsValue[i] = new List<(int, int)>();
        }

        for (var y = 0; y < _bitmap.Height; y++)
        {
            for (var x = 0; x < _bitmap.Width; x++)
            {
                var pixel = _bitmap.GetPixel(x, y);

                pixelsValue[pixel.Red + pixel.Green + pixel.Blue].Add((x, y));
            }
        }

        return pixelsValue;
    }

    private List<(int, int)> GetIgnorePixel(IReadOnlyList<List<(int, int)>> pixelsValue, double ignorePercent)
    {
        var ignorePixels = new List<(int, int)>();
        var ignorePixelsCount = (int)(_bitmap.Height * _bitmap.Width * ignorePercent);

        for (var i = 0; i < 766; i++)
        {
            for (var j = 0; j < pixelsValue[i].Count; j++)
            {
                if (ignorePixelsCount == 0)
                    break;

                ignorePixels.Add(pixelsValue[i][j]);
                ignorePixelsCount--;
            }

            if (ignorePixelsCount == 0)
                break;
        }

        ignorePixelsCount = (int)(_bitmap.Height * _bitmap.Width * ignorePercent);

        for (var i = 765; i >= 0; i--)
        {
            for (var j = 0; j < pixelsValue[i].Count; j++)
            {
                if (ignorePixelsCount == 0)
                    break;

                ignorePixels.Add(pixelsValue[i][j]);
                ignorePixelsCount--;
            }

            if (ignorePixelsCount == 0)
                break;
        }

        return ignorePixels;
    }

    private int GetMinValue()
    {
        for (var i = 0; i < 256; i++)
        {
            if ((_redHistogram != null && _redHistogram[i].Count != 0) ||
                (_greenHistogram != null && _greenHistogram[i].Count != 0) ||
                (_blueHistogram != null && _blueHistogram[i].Count != 0))
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
            if ((_redHistogram != null && _redHistogram[i].Count != 0) ||
                (_greenHistogram != null && _greenHistogram[i].Count != 0) ||
                (_blueHistogram != null && _blueHistogram[i].Count != 0))
            {
                return i;
            }
        }

        throw new Exception("Data Error");
    }

    private int Increment(int index)
    {
        if (_redHistogram != null || _greenHistogram != null || _blueHistogram != null)
            return index + 1;

        return index;
    }
}