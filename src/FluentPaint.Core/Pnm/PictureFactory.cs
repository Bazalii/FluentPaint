using FluentPaint.Core.Reader;
using FluentPaint.Core.Reader.Implementations;
using FluentPaint.Core.Writer;
using FluentPaint.Core.Writer.Implementations;

namespace FluentPaint.Core.Pnm;

/// <summary>
/// Provides writers and readers of files in all supported formats.
/// </summary>
public static class PictureFactory
{
    /// <summary>
    /// Picks up correct format of the picture using string.
    /// </summary>
    /// <param name="line"> contains format of the picture. </param>
    /// <returns>
    /// Proper type of the picture
    /// </returns>
    public static PictureType GetType(string line)
    {
        return line.Equals("P5") ? PictureType.P5 : PictureType.P6;
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
            _ => throw new ArgumentException("Error: This file type is not supported, .ppm .pgm is expected")
        };

        return reader;
    }
}