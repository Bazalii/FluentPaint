using SkiaSharp;

namespace FluentPaint.Core.Reader;

public interface IPictureReader
{
    SKBitmap ReadImageData(FileStream fileStream, int width, int height);
}