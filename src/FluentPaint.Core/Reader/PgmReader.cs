using SkiaSharp;

namespace FluentPaint.Core.Reader;

public class PgmReader: IPnmReader
{
    public SKBitmap ReadImageData(FileStream fs, int width, int height)
    {
        var im = new SKBitmap(width, height);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var val = (byte) fs.ReadByte();
                im.SetPixel(x, y, new SKColor(val, val, val));
            }
        }

        return im;
    }
}