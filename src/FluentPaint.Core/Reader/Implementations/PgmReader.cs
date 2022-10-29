using SkiaSharp;

namespace FluentPaint.Core.Reader.Implementations;

/// <summary>
/// Reader for .pgm files.
/// </summary>
public class PgmReader : IPictureReader
{
    public SKBitmap ReadImageData(FileStream fileStream, int width, int height)
    {
        var bitmap = new SKBitmap(width, height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var value = (byte)fileStream.ReadByte();

                bitmap.SetPixel(x, y, new SKColor(value, value, value));
            }
        }

        return bitmap;
    }
}