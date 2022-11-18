using SkiaSharp;

namespace FluentPaint.Core.Pictures.Readers.Implementations;

public class JpegReader : IPictureReader
{
    public SKBitmap ReadImageData(FileStream fileStream)
    {
        return new SKBitmap();
    }
}