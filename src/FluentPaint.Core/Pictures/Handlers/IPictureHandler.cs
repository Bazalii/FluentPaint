using SkiaSharp;

namespace FluentPaint.Core.Pictures.Handlers;

public interface IPictureHandler
{
    SKBitmap Read(string filePath);
    void Write(string filePath, SKBitmap bitmap);
}