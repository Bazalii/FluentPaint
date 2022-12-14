using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.Writers.Implementations;

public class PngWriter : IPictureWriter
{
    public void WriteImageData(FileStream fileStream, SKBitmap bitmap, PictureType type)
    {
        throw new NotImplementedException();
    }
}