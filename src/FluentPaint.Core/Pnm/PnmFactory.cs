using FluentPaint.Core.Pnm;
using FluentPaint.Core.Reader;
using FluentPaint.Core.Writer;

namespace FluentPaint.Core.Pnm;

public static class PnmFactory
{
    public static PnmType GetPnmType(string line)
    {
        return line.Equals("P5") ? PnmType.P5 : PnmType.P6;
    }
    
    public static IPnmWriter GetIPnmWriter(PnmType type)
    {
        IPnmWriter imWriter = type switch
        {
            PnmType.P5 => new PgmWriter(),
            PnmType.P6 => new PpmWriter(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        return imWriter;
    }
    
    
    public static IPnmReader GetIPnmReader(PnmType type)
    {
        IPnmReader imWriter = type switch
        {
            PnmType.P5 => new PgmReader(),
            PnmType.P6 => new PpmReader(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        return imWriter;
    }
}