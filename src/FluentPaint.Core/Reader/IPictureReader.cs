using SkiaSharp;

namespace FluentPaint.Core.Reader;

/// <summary>
/// Default interface for readers.
/// </summary>
public interface IPictureReader
{
    /// <summary>
    /// Read picture from file.
    /// </summary>
    /// <param name="fileStream"> file with picture. </param>
    /// <param name="width"> picture width, the number of bytes in the row. </param>
    /// <param name="height"> picture height, the number of bytes in the column. </param>
    /// <returns>filled bitmap. </returns>
    SKBitmap ReadImageData(FileStream fileStream, int width, int height);
}