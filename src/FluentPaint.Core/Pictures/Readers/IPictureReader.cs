using SkiaSharp;

namespace FluentPaint.Core.Pictures.Readers;

/// <summary>
/// Default interface for readers.
/// </summary>
public interface IPictureReader
{
    /// <summary>
    /// Read picture from file.
    /// </summary>
    /// <param name="fileStream"> file with picture. </param>
    /// <returns>
    /// Filled bitmap.
    /// </returns>
    FluentBitmap ReadImageData(FileStream fileStream);
}