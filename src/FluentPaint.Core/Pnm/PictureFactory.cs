using FluentPaint.Core.Reader;
using FluentPaint.Core.Reader.Implementations;
using FluentPaint.Core.Writer;
using FluentPaint.Core.Writer.Implementations;

namespace FluentPaint.Core.Pnm;

public static class PictureFactory
{
    public static PictureType GetType(string line)
    {
        return line.Equals("P5") ? PictureType.P5 : PictureType.P6;
    }

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