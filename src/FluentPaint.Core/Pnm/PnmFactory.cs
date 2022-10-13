using FluentPaint.Core.Reader;
using FluentPaint.Core.Writer;

namespace FluentPaint.Core.Pnm;

public static class PnmFactory
{
    public static PnmType GetPnmType(string line)
    {
        return line.Equals("P5") ? PnmType.P5 : PnmType.P6;
    }
    
    public static IPnmWriter GetPnmWriter(PnmType type)
    {
        IPnmWriter writer = type switch
        {
            PnmType.P5 => new PgmWriter(),
            PnmType.P6 => new PpmWriter(),
            _ => throw new Exception("Error: This file type is not supported, .ppm .pgm is expected")
        };

        return writer;
    }
    
    
    public static IPnmReader GetPnmReader(PnmType type)
    {
        IPnmReader reader = type switch
        {
            PnmType.P5 => new PgmReader(),
            PnmType.P6 => new PpmReader(),
            _ => throw new Exception("Error: This file type is not supported, .ppm .pgm is expected")
        };

        return reader;
    }
}