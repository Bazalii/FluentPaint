using SkiaSharp;

namespace FluentPaint.Core.Reader;

public class PpmReader: IPnmReader
{
    public SKBitmap ReadImageData(FileStream fs, int width, int height)
    {
        var im = new SKBitmap(width, height);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var valR = (byte) fs.ReadByte();
                var valG = (byte) fs.ReadByte();
                var valB = (byte) fs.ReadByte();
                im.SetPixel(x, y, new SKColor(valR, valG, valB));
            }
        }

        return im;
    }
}