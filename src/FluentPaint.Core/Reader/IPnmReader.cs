using SkiaSharp;

namespace FluentPaint.Core.Reader;

public interface IPnmReader
{
    SKBitmap ReadImageData(FileStream fs, int width, int height);
}