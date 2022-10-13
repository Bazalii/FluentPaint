using SkiaSharp;

namespace FluentPaint.Core.Reader.Implementations;

public class PgmReader: IPnmReader
{
    public SKBitmap ReadImageData(FileStream fileStream, int width, int height)
    {
        var bitmap = new SKBitmap(width, height);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var val = (byte) fileStream.ReadByte();
                bitmap.SetPixel(x, y, new SKColor(val, val, val));
            }
        }

        return bitmap;
    }
}