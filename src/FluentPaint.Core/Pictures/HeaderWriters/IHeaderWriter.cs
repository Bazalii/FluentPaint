using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.HeaderWriters;

public interface IHeaderWriter
{
    void Write(FileStream fileStream, SKBitmap bitmap, PictureType type);
}