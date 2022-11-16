using SkiaSharp;

namespace FluentPaint.Core.Pictures.Handlers.Implementations;

/// <summary>
/// Provides methods to read and write pnm picture using filepath.
/// </summary>
public class PictureHandler : IPictureHandler
{
    /// <summary>
    /// Reads pnm file using filepath and writes it to Bitmap.
    /// </summary>
    /// <param name="filePath"> absolute filepath to the picture. </param>
    /// <exception cref="Exception">
    /// Thrown when brightness value not equal to 255.
    /// </exception>
    /// <returns>
    /// <see cref="SKBitmap"/> with read pixels.
    /// </returns>
    public SKBitmap Read(string filePath)
    {
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var type = PictureFactory.GetType(filePath);

        var reader = PictureFactory.GetReader(type);

        return reader.ReadImageData(fileStream);
    }

    /// <summary>
    /// Writes pnm file from bitmap in file using provided filepath.
    /// </summary>
    /// <param name="filePath"> an absolute filepath to the picture. </param>
    /// <param name="bitmap"> a data structure that contains all bytes of the picture. </param>
    /// <exception cref="Exception">
    /// Thrown when provided picture format is incorrect.
    /// </exception>
    public void Write(string filePath, SKBitmap bitmap)
    {
        var type = PictureFactory.GetType(filePath);

        var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        var writer = PictureFactory.GetWriter(type);

        writer.WriteImageData(fileStream, bitmap, type);
    }
}