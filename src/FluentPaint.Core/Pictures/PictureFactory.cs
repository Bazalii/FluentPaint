using FluentPaint.Core.Enums;
using FluentPaint.Core.Pictures.Readers;
using FluentPaint.Core.Pictures.Readers.Implementations;
using FluentPaint.Core.Pictures.Writers;
using FluentPaint.Core.Pictures.Writers.Implementations;

namespace FluentPaint.Core.Pictures;

/// <summary>
/// Provides writers and readers of files in all supported formats.
/// </summary>
public static class PictureFactory
{
    /// <summary>
    /// Picks up correct format of the picture using string.
    /// </summary>
    /// <param name="filePath"> path to some file. </param>
    /// <returns>
    /// Proper type of the picture
    /// </returns>
    public static PictureType GetType(string filePath)
    {
        var indexOfDot = filePath.IndexOf(".", StringComparison.Ordinal);

        var extension = filePath.Substring(indexOfDot, filePath.Length - indexOfDot).ToLower();

        return extension switch
        {
            ".pgm" => PictureType.P5,
            ".ppm" => PictureType.P6,
            ".pnm" => PictureType.P6,
            ".jpeg" => PictureType.JPEG,
            _ => throw new Exception(
                "Error: This file type is not supported! .ppm, .pgm and .jpeg are expected (.pnm will write as p6)")
        };
    }

    /// <summary>
    /// Picks up suitable writer using a picture format.
    /// </summary>
    /// <param name="type"> type of the picture. </param>
    /// <returns>
    /// Object of <see cref="IPictureWriter"/> type to handle pictures of provided type.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when provided picture format is incorrect.
    /// </exception>
    public static IPictureWriter GetWriter(PictureType type)
    {
        IPictureWriter writer = type switch
        {
            PictureType.P5 => new PgmWriter(),
            PictureType.P6 => new PpmWriter(),
            PictureType.JPEG => new JpegWriter(),
            _ => throw new ArgumentException("Error: This file type is not supported, .ppm .pgm is expected")
        };

        return writer;
    }

    /// <summary>
    /// Picks up suitable reader using picture format.
    /// </summary>
    /// <param name="type"> type of the picture. </param>
    /// <returns>
    /// Object of <see cref="IPictureReader"/> type to handle pictures of provided type.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when provided picture format is incorrect.
    /// </exception>
    public static IPictureReader GetReader(PictureType type)
    {
        IPictureReader reader = type switch
        {
            PictureType.P5 => new PgmReader(),
            PictureType.P6 => new PpmReader(),
            PictureType.JPEG => new JpegReader(),
            _ => throw new ArgumentException("Error: This file type is not supported, .ppm .pgm is expected")
        };

        return reader;
    }
}