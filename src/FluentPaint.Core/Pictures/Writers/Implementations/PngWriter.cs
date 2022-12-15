using System.Text;
using FluentPaint.Core.Enums;
using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.Writers.Implementations;

public class PngWriter : IPictureWriter
{
    private const byte BitDepth = 8;
    private const byte ColorType = 2;
    private const byte CompressionMethod = 0;
    private const byte FilterMethod = 0;
    private const byte InterlaceMethod = 0;
    private const byte BytesPerPixel = 3;

    private FileStream _fileStream;
    private SKBitmap _bitmap;

    public void WriteImageData(FileStream fileStream, SKBitmap bitmap, PictureType type)
    {
        _fileStream = fileStream;
        _bitmap = bitmap;

        WriteSignature();
        WriteHeader();
    }

    private void WriteSignature()
    {
        var signature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
        
        _fileStream.Write(signature);
    }

    private void WriteHeader()
    {
        var sectionName = new ASCIIEncoding().GetBytes("IHDR");

        var section = new List<byte>();

        section.AddRange(sectionName);
        section.AddRange(BitConverter.GetBytes(_bitmap.Width).Reverse());
        section.AddRange(BitConverter.GetBytes(_bitmap.Height).Reverse());
        section.Add(BitDepth);
        section.Add(ColorType);
        section.Add(CompressionMethod);
        section.Add(FilterMethod);
        section.Add(InterlaceMethod);

        var sectionLength = BitConverter.GetBytes(section.Count - 4).Reverse();

        var cyclicRedundancyCodeCalculator = new Crc32();
        cyclicRedundancyCodeCalculator.Update(section.ToArray());

        var cyclicRedundancyCode = BitConverter.GetBytes((int) cyclicRedundancyCodeCalculator.Value).Reverse();

        _fileStream.Write(sectionLength.ToArray());
        _fileStream.Write(section.ToArray());
        _fileStream.Write(cyclicRedundancyCode.ToArray());
    }
}