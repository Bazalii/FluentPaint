using SkiaSharp;

namespace FluentPaint.Core.Pictures.Handlers;

public interface IPictureHandler
{
    FluentBitmap Read(string filePath);
    void Write(string filePath, FluentBitmap bitmap);
}