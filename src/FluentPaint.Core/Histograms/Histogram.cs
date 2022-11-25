using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Histograms;

public class Histogram
{
    public ColorChannels Channels { get; }

    public int StartIndex { get; private set; }
    public int EndIndex { get; private set; }

    public int[]? RedHistogram;
    public int[]? GreenHistogram;
    public int[]? BlueHistogram;

    private SKBitmap _bitmap;

    public Histogram(ColorChannels channels, int startIndex = 0, int endIndex = 255)
    {
        Channels = channels;
        StartIndex = startIndex;
        EndIndex = endIndex;

        switch (channels)
        {
            case ColorChannels.First:
                RedHistogram = new int[256];
                break;
            case ColorChannels.Second:
                GreenHistogram = new int[256];
                break;
            case ColorChannels.Third:
                BlueHistogram = new int[256];
                break;
            case ColorChannels.FirstAndSecond:
                RedHistogram = new int[256];
                GreenHistogram = new int[256];
                break;
            case ColorChannels.FirstAndThird:
                RedHistogram = new int[256];
                BlueHistogram = new int[256];
                break;
            case ColorChannels.SecondAndThird:
                GreenHistogram = new int[256];
                BlueHistogram = new int[256];
                break;
            case ColorChannels.All:
                RedHistogram = new int[256];
                GreenHistogram = new int[256];
                BlueHistogram = new int[256];
                break;
        }
    }

    public void CreateHistograms(SKBitmap bitmap, double ignorePercent)
    {
        _bitmap = bitmap;
        
        StartIndex = (int)(256 * ignorePercent);
        EndIndex = (int)(255 - 256 * ignorePercent);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                if (RedHistogram != null && StartIndex <= pixel.Red && pixel.Red <= EndIndex)
                    RedHistogram[pixel.Red]++;
                if (GreenHistogram != null && StartIndex <= pixel.Green && pixel.Green <= EndIndex)
                    GreenHistogram[pixel.Green]++;
                if (BlueHistogram != null && StartIndex <= pixel.Blue && pixel.Blue <= EndIndex)
                    BlueHistogram[pixel.Blue]++;
            }
        }
    }
    
    public SKBitmap AutomaticCorrection()
    {
        var resultHistogram = new Histogram(Channels, StartIndex, EndIndex);

        var unusedValues = new List<int>();

        for (var i = StartIndex; i <= EndIndex; i++)
        {
            if (IsUnused(i))
            {
                unusedValues.Add(i);
            }
        }

        var redIndex = 0;
        var greenIndex = 0;
        var blueIndex = 0;
        var compressionHistogram = new Histogram(Channels, StartIndex, EndIndex);

        for (var i = StartIndex; i <= EndIndex; i++)
        {
            if (unusedValues.Any(value => value == i)) continue;

            if (RedHistogram != null)
                compressionHistogram.RedHistogram![redIndex++] = RedHistogram[i];
            if (GreenHistogram != null)
                compressionHistogram.GreenHistogram![greenIndex++] = GreenHistogram[i];
            if (BlueHistogram != null)
                compressionHistogram.BlueHistogram![blueIndex++] = BlueHistogram[i];
        }

        var index = StartIndex;
        var compressionIndex = StartIndex;
        var size = EndIndex - StartIndex + 1;
        double correction = 0;

        while (index <= EndIndex)
        {
            correction += (size - unusedValues.Count) / (unusedValues.Count + 1);
            while (correction > 1)
            {
                if (compressionHistogram.RedHistogram != null)
                    resultHistogram.RedHistogram![index++] = compressionHistogram.RedHistogram[compressionIndex++];
                if (compressionHistogram.GreenHistogram != null)
                    resultHistogram.GreenHistogram![index++] = compressionHistogram.GreenHistogram[compressionIndex++];
                if (compressionHistogram.BlueHistogram != null)
                    resultHistogram.BlueHistogram![index++] = compressionHistogram.BlueHistogram[compressionIndex++];
                correction -= 1;
            }

            if (index == EndIndex && correction > 0)
            {
                if (compressionHistogram.RedHistogram != null)
                    resultHistogram.RedHistogram![index++] = compressionHistogram.RedHistogram[compressionIndex++];
                if (compressionHistogram.GreenHistogram != null)
                    resultHistogram.GreenHistogram![index++] = compressionHistogram.GreenHistogram[compressionIndex++];
                if (compressionHistogram.BlueHistogram != null)
                    resultHistogram.BlueHistogram![index++] = compressionHistogram.BlueHistogram[compressionIndex++];
            }
            else
            {
                if (compressionHistogram.RedHistogram != null)
                    resultHistogram.RedHistogram![index++] = 0;
                if (compressionHistogram.GreenHistogram != null)
                    resultHistogram.GreenHistogram![index++] = 0;
                if (compressionHistogram.BlueHistogram != null)
                    resultHistogram.BlueHistogram![index++] = 0;
            }
        }

        var resultBitmap = new SKBitmap();
        
        var oldIndex = StartIndex;
        var newIndex = StartIndex;

        while (oldIndex <= EndIndex || newIndex <= EndIndex)
        {
            while (IsUnused(oldIndex))
                oldIndex++;
            while (resultHistogram.IsUnused(newIndex))
                newIndex++;

            SetNewPixels(_bitmap, ref resultBitmap, oldIndex, newIndex);
        }
        
        RedHistogram = resultHistogram.RedHistogram;
        BlueHistogram = resultHistogram.BlueHistogram;
        GreenHistogram = resultHistogram.GreenHistogram;
        _bitmap = resultBitmap;
        return _bitmap;
    }

    private bool IsUnused(int index)
    {
        return (RedHistogram == null || RedHistogram[index] == 0) &&
               (GreenHistogram == null || GreenHistogram[index] == 0) &&
               (BlueHistogram == null || BlueHistogram[index] == 0);
    }
    
    private void SetNewPixels(SKBitmap oldBitmap, ref SKBitmap newBitmap, int oldValue, int newValue)
    {
        for (var y = 0; y < oldBitmap.Height; y++)
        {
            for (var x = 0; x < oldBitmap.Width; x++)
            {
                var pixel = oldBitmap.GetPixel(x, y);

                if (pixel.Red == oldValue)
                    newBitmap.SetPixel(x, y, new SKColor((byte)newValue, pixel.Green, pixel.Blue));
                if(pixel.Green == oldValue)
                    newBitmap.SetPixel(x, y, new SKColor(pixel.Red, (byte)newValue, pixel.Blue));
                if(pixel.Blue == oldValue)
                    newBitmap.SetPixel(x, y, new SKColor(pixel.Red, pixel.Green, (byte)newValue));
            }
        }
    }
}