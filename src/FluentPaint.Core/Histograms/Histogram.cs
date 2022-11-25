using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Histograms;

public class Histogram
{
    private int[]? _redHistogram;
    private int[]? _greenHistogram;
    private int[]? _blueHistogram;

    private SKBitmap _bitmap;
    private List<(int, int)> _ignorePixels = new();

    public Histogram(ColorChannels channels, SKBitmap bitmap)
    {
        _bitmap = bitmap;

        switch (channels)
        {
            case ColorChannels.First:
                _redHistogram = new int[256];
                break;
            case ColorChannels.Second:
                _greenHistogram = new int[256];
                break;
            case ColorChannels.Third:
                _blueHistogram = new int[256];
                break;
            case ColorChannels.FirstAndSecond:
                _redHistogram = new int[256];
                _greenHistogram = new int[256];
                break;
            case ColorChannels.FirstAndThird:
                _redHistogram = new int[256];
                _blueHistogram = new int[256];
                break;
            case ColorChannels.SecondAndThird:
                _greenHistogram = new int[256];
                _blueHistogram = new int[256];
                break;
            case ColorChannels.All:
                _redHistogram = new int[256];
                _greenHistogram = new int[256];
                _blueHistogram = new int[256];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(channels), channels, null);
        }
    }

    public void CreateHistograms(double ignorePercent)
    {
        var pixelsValue = CreatePixelsValue();

        if (ignorePercent != 0)
            SetIgnorePixel(pixelsValue, ignorePercent);

        for (var y = 0; y < _bitmap.Height; y++)
        {
            for (var x = 0; x < _bitmap.Width; x++)
            {
                var pixel = _bitmap.GetPixel(x, y);

                if (_redHistogram != null && _ignorePixels.All(tuple => tuple != (x, y)))
                    _redHistogram[pixel.Red]++;
                if (_greenHistogram != null && _ignorePixels.All(tuple => tuple != (x, y)))
                    _greenHistogram[pixel.Green]++;
                if (_blueHistogram != null && _ignorePixels.All(tuple => tuple != (x, y)))
                    _blueHistogram[pixel.Blue]++;
            }
        }
    }

    public SKBitmap AutomaticCorrection()
    {
        var resultBitmap = new SKBitmap(_bitmap.Width, _bitmap.Height);
        var redHistogram = _redHistogram == null ? null : new int[256];
        var greenHistogram = _greenHistogram == null ? null : new int[256];
        var blueHistogram = _blueHistogram == null ? null : new int[256];

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
                redHistogram[newIndex] = 0;
            if (greenHistogram != null)
                greenHistogram[newIndex] = 0;
            if (blueHistogram != null)
                blueHistogram[newIndex] = 0;
            newIndex = Increment(newIndex);
        }

        oldIndex = min;

        for (var i = 0; i < 256; i++)
        {
            if(!((redHistogram != null && redHistogram[i] != 0) ||
               (greenHistogram != null && greenHistogram[i] != 0) ||
               (blueHistogram != null && blueHistogram[i] != 0)))
                continue;
            
            for (var y = 0; y < _bitmap.Height; y++)
            {
                for (var x = 0; x < _bitmap.Width; x++)
                {
                    if(_ignorePixels.Any(tuple => tuple == (x, y)))
                        continue;
                    
                    var pixel = _bitmap.GetPixel(x, y);
                    var newPixel = resultBitmap.GetPixel(x, y);

                    if (pixel.Red == oldIndex)
                    {
                        resultBitmap.SetPixel(x, y, new SKColor((byte)i, newPixel.Green, newPixel.Blue));
                        newPixel = resultBitmap.GetPixel(x, y);
                    }
                    if(pixel.Green == oldIndex)
                    {
                        resultBitmap.SetPixel(x, y, new SKColor(newPixel.Red, (byte)i, newPixel.Blue));
                        newPixel = resultBitmap.GetPixel(x, y);
                    }
                    if(pixel.Blue == oldIndex)
                        resultBitmap.SetPixel(x, y, new SKColor(newPixel.Red, newPixel.Green, (byte)i));
                }
            }

            oldIndex++;
        }

        _redHistogram = redHistogram;
        _greenHistogram = greenHistogram;
        _blueHistogram = blueHistogram;
        _bitmap = resultBitmap;

        return resultBitmap;
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

    private void SetIgnorePixel(IReadOnlyList<List<(int, int)>> pixelsValue, double ignorePercent)
    {
        _ignorePixels = new List<(int, int)>();
        var ignorePixelsCount = (int)(_bitmap.Height * _bitmap.Width * ignorePercent);

        for (var i = 0; i < 766; i++)
        {
            for (var j = 0; j < pixelsValue[i].Count; j++)
            {
                if (ignorePixelsCount == 0)
                    break;

                _ignorePixels.Add(pixelsValue[i][j]);
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

                _ignorePixels.Add(pixelsValue[i][j]);
                ignorePixelsCount--;
            }

            if (ignorePixelsCount == 0)
                break;
        }
    }

    private int GetMinValue()
    {
        for (var i = 0; i < 256; i++)
        {
            if ((_redHistogram != null && _redHistogram[i] != 0) ||
                (_greenHistogram != null && _greenHistogram[i] != 0) ||
                (_blueHistogram != null && _blueHistogram[i] != 0))
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
            if ((_redHistogram != null && _redHistogram[i] != 0) ||
                (_greenHistogram != null && _greenHistogram[i] != 0) ||
                (_blueHistogram != null && _blueHistogram[i] != 0))
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