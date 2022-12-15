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
        WriteImage();
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

    private void WriteImage()
    {
        var encodedImageData = new byte[_bitmap.Height * (1 + _bitmap.Width * BytesPerPixel)];

        var imagePointer = 0;

        for (var y = 0; y < _bitmap.Height; y++)
        {
            encodedImageData[0] = 0;

            imagePointer += 1;

            for (var x = 0; x < _bitmap.Width; x++)
            {
                var currentPixel = _bitmap.GetPixel(x, y);

                encodedImageData[imagePointer] = currentPixel.Red;
                encodedImageData[imagePointer + 1] = currentPixel.Green;
                encodedImageData[imagePointer + 2] = currentPixel.Blue;

                imagePointer += 3;
            }
        }

        var outputStream = new MemoryStream();
        var compressStream = new DeflaterOutputStream(outputStream);
        compressStream.Write(encodedImageData);
        compressStream.Flush();
        compressStream.Finish();

        var sectionName = new ASCIIEncoding().GetBytes("IDAT");

        var section = new List<byte>();

        section.AddRange(sectionName);
        section.AddRange(outputStream.ToArray());

        var sectionLength = BitConverter.GetBytes(section.Count - 4).Reverse();

        var imageSectionCyclicRedundancyCodeCalculator = new Crc32();
        imageSectionCyclicRedundancyCodeCalculator.Update(section.ToArray());
        var imageSectionRedundancyCode =
            BitConverter.GetBytes((int) imageSectionCyclicRedundancyCodeCalculator.Value).Reverse();
        
        var endFileSectionName = new ASCIIEncoding().GetBytes("IEND");
        var endFileSectionLength = BitConverter.GetBytes(0);
        
        var endFileSectionCyclicRedundancyCodeCalculator = new Crc32();
        endFileSectionCyclicRedundancyCodeCalculator.Update(endFileSectionName);
        var endFileSectionCyclicRedundancyCode =
            BitConverter.GetBytes((int) endFileSectionCyclicRedundancyCodeCalculator.Value).Reverse();

        _fileStream.Write(sectionLength.ToArray());
        _fileStream.Write(section.ToArray());
        _fileStream.Write(imageSectionRedundancyCode.ToArray());

        _fileStream.Write(endFileSectionLength);
        _fileStream.Write(endFileSectionName);
        _fileStream.Write(endFileSectionCyclicRedundancyCode.ToArray());
        
        _fileStream.Close();
    }
}