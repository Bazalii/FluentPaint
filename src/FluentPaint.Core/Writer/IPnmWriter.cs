using SkiaSharp;

namespace FluentPaint.Core.Writer;

public interface IPnmWriter
{
    void WriteImageData(FileStream fileStream, SKBitmap bitmap);
}