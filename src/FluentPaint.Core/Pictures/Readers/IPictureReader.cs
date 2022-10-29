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
    /// <param name="width"> picture width, which is the number of bytes in the row. </param>
    /// <param name="height"> picture height, which is the number of bytes in the column. </param>
    /// <returns>
    /// Filled bitmap.
    /// </returns>
    SKBitmap ReadImageData(FileStream fileStream, int width, int height);
}