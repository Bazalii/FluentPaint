using SkiaSharp;

namespace FluentPaint.Core.Reader;

public interface IPnmReader
{
    SKBitmap ReadImageData(FileStream fileStream, int width, int height);
}