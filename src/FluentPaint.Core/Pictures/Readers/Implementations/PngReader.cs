using System.Text;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.Readers.Implementations;

public class PngReader : IPictureReader
{
    private int _width;
    private int _height;
    private int _bitDepth;
    private int _colorType;

    public SKBitmap ReadImageData(FileStream fileStream)
    {
        var buffer = new byte[8];
        fileStream.Read(buffer);

        buffer = new byte[4];
        var pointer = 8;

        var bitmap = new SKBitmap();

        while (pointer < fileStream.Length)
        {
            fileStream.Read(buffer);
            var currentSectionLength = int.Parse(Convert.ToHexString(buffer),
                System.Globalization.NumberStyles.HexNumber);

            fileStream.Read(buffer);
            var currentSection = new ASCIIEncoding().GetString(buffer);

            switch (currentSection)
            {
                case "IHDR":
                    var sectionBuffer = new byte[currentSectionLength];

                    fileStream.Read(sectionBuffer);

                    ReadHeader(sectionBuffer);
                    break;
                case "PLTE":
                    break;
                case "IDAT":
                    break;
                case "gAMA":
                    break;
                case "IEND":
                    break;
            }
        }

        return bitmap;
    }

    private void ReadHeader(byte[] section)
    {
        _width = int.Parse(Convert.ToHexString(section[..4]), System.Globalization.NumberStyles.HexNumber);
        _height = int.Parse(Convert.ToHexString(section[4..8]), System.Globalization.NumberStyles.HexNumber);
        _bitDepth = int.Parse(Convert.ToHexString(new[] { section[8] }), System.Globalization.NumberStyles.HexNumber);
        _colorType = int.Parse(Convert.ToHexString(new[] { section[9] }), System.Globalization.NumberStyles.HexNumber);
        var compressionMethod = int.Parse(Convert.ToHexString(new[] { section[10] }), System.Globalization.NumberStyles.HexNumber);
        var filterMethod = int.Parse(Convert.ToHexString(new[] { section[10] }), System.Globalization.NumberStyles.HexNumber);
        var interlaceMethod = int.Parse(Convert.ToHexString(new[] { section[10] }), System.Globalization.NumberStyles.HexNumber);

        if (_bitDepth != 8)
        {
            throw new Exception("Bit depth should be 8!");
        }
        
        if (compressionMethod != 0)
        {
            throw new Exception("Only deflate is allowed!");
        }
        
        if (filterMethod != 0)
        {
            throw new Exception("Only adaptive filtering is allowed!");
        }
        
        if (interlaceMethod != 0)
        {
            throw new Exception("Interlacing is not allowed!");
        }
    }
}