using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.Writers;

/// <summary>
/// Default interface for writers.
/// </summary>
public interface IPictureWriter
{
    /// <summary>
    /// Write picture in a file.
    /// Then close fileStream.
    /// </summary>
    /// <param name="fileStream"> file into which you want to write the picture. </param>
    /// <param name="bitmap"> bitmap with picture. </param>
    /// <param name="type"> file type. </param>
    void WriteImageData(FileStream fileStream, FluentBitmap bitmap, PictureType type);
}