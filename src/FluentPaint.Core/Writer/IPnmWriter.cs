using SkiaSharp;

namespace FluentPaint.Core.Writer;

public interface IPnmWriter
{
    public void WriteImageData(FileStream fs, SKBitmap im);
}