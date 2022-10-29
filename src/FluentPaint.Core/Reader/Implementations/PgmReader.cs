using SkiaSharp;

namespace FluentPaint.Core.Reader.Implementations;

/// <summary>
/// Reader for .pgm files
/// </summary>
public class PgmReader : IPictureReader
{
    public SKBitmap ReadImageData(FileStream fileStream, int width, int height)
    {
        var bitmap = new SKBitmap(width, height);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var value = (byte)fileStream.ReadByte();

                bitmap.SetPixel(x, y, new SKColor(value, value, value));
            }
        }

        return bitmap;
    }
}