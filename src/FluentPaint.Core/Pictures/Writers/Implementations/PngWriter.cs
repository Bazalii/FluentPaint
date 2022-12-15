using System.Text;
using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.Writers.Implementations;

public class PngWriter : IPictureWriter
{
    private const byte BitDepth = 8;
    private const byte ColorType = 2;
    private const byte CompressionMethod = 0;
    private const byte FilterMethod = 0;
    private const byte InterlaceMethod = 0;
    
    private FileStream _fileStream;
    private SKBitmap _bitmap;
    
    public void WriteImageData(FileStream fileStream, SKBitmap bitmap, PictureType type)
    {
        _fileStream = fileStream;
        _bitmap = bitmap;
        
        WriteHeader();
    }
    
    private void WriteHeader()
    {
        var sectionName = new ASCIIEncoding().GetBytes("IHDR");
        
        _fileStream.Write(sectionName);
        _fileStream.Write(BitConverter.GetBytes(_bitmap.Width));
        _fileStream.Write(BitConverter.GetBytes(_bitmap.Height));
        _fileStream.WriteByte(BitDepth);
        _fileStream.WriteByte(ColorType);
        _fileStream.WriteByte(CompressionMethod);
        _fileStream.WriteByte(FilterMethod);
        _fileStream.WriteByte(InterlaceMethod);
    }
}