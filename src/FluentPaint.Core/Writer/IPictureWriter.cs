using SkiaSharp;

namespace FluentPaint.Core.Writer;

public interface IPictureWriter
{
    void WriteImageData(FileStream fileStream, SKBitmap bitmap);
}