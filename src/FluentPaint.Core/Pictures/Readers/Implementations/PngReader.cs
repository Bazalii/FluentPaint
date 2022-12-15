using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.Readers.Implementations;

public class PngReader : IPictureReader
{
    private int _bytesPerPixel = 3;
    private int _width;
    private int _height;
    private int _bitDepth;
    private int _colorType;
    private int _stride;
    private List<byte> _encodedImage = new();
    private List<byte> _decodedImage = new();

    public SKBitmap ReadImageData(FileStream fileStream)
    {
        var buffer = new byte[8];
        fileStream.Read(buffer);

        buffer = new byte[4];

        while (true)
        {
            fileStream.Read(buffer);
            var currentSectionLength = int.Parse(Convert.ToHexString(buffer),
                System.Globalization.NumberStyles.HexNumber);

            fileStream.Read(buffer);
            var currentSection = new ASCIIEncoding().GetString(buffer);

            var sectionBuffer = new byte[currentSectionLength];
            fileStream.Read(sectionBuffer);

            var cyclicRedundancyCodeBuffer = new byte[4];
            fileStream.Read(cyclicRedundancyCodeBuffer);

            switch (currentSection)
            {
                case "IHDR":
                    ReadHeader(sectionBuffer);
                    break;
                case "IDAT":
                    _encodedImage.AddRange(sectionBuffer);
                    break;
                case "gAMA":
                    break;
                case "IEND":
                    break;
            }
        }

        return null;
    }

    private void ReadHeader(byte[] section)
    {
        _width = int.Parse(Convert.ToHexString(section[..4]), System.Globalization.NumberStyles.HexNumber);
        _height = int.Parse(Convert.ToHexString(section[4..8]), System.Globalization.NumberStyles.HexNumber);
        _bitDepth = int.Parse(Convert.ToHexString(new[] { section[8] }), System.Globalization.NumberStyles.HexNumber);
        _colorType = int.Parse(Convert.ToHexString(new[] { section[9] }), System.Globalization.NumberStyles.HexNumber);
        var compressionMethod = int.Parse(Convert.ToHexString(new[] { section[10] }),
            System.Globalization.NumberStyles.HexNumber);
        var filterMethod = int.Parse(Convert.ToHexString(new[] { section[10] }),
            System.Globalization.NumberStyles.HexNumber);
        var interlaceMethod = int.Parse(Convert.ToHexString(new[] { section[10] }),
            System.Globalization.NumberStyles.HexNumber);

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

    private void GetImage(byte[] imageData)
    {
        var imagePointer = 0;

        for (var i = 0; i < _height; i++)
        {
            var filter = imageData[imagePointer];
            imagePointer += 1;

            for (var j = 0; j < _stride; j++)
            {
                var filteredByte = imageData[imagePointer];
                imagePointer += 1;

                var reconstructedPixel = filter switch
                {
                    0 => filteredByte,
                    1 => filteredByte + ProcessFirstReconstruction(i, j),
                    2 => filteredByte + ProcessSecondReconstruction(i, j),
                    3 => filteredByte + (ProcessFirstReconstruction(i, j) + ProcessSecondReconstruction(i, j)) / 2,
                    4 => filteredByte + Predict(ProcessFirstReconstruction(i, j), ProcessSecondReconstruction(i, j), ProcessThirdReconstruction(i, j)),
                    _ => throw new Exception("Filter is not supported!")
                };

                _decodedImage.Add((byte) (reconstructedPixel & 0xff));
            }
        }
    }

    private byte ProcessFirstReconstruction(int reconstructedIndex, int alongScanlineIndex)
    {
        return alongScanlineIndex >= _bytesPerPixel
            ? _decodedImage[reconstructedIndex * _stride + alongScanlineIndex - _bytesPerPixel]
            : (byte) 0;
    }

    private byte ProcessSecondReconstruction(int reconstructedIndex, int alongScanlineIndex)
    {
        return reconstructedIndex > 0
            ? _decodedImage[(reconstructedIndex - 1) * _stride + alongScanlineIndex]
            : (byte) 0;
    }

    private byte ProcessThirdReconstruction(int reconstructedIndex, int alongScanlineIndex)
    {
        return reconstructedIndex > 0 && alongScanlineIndex >= _bytesPerPixel
            ? _decodedImage[(reconstructedIndex - 1) * _stride + alongScanlineIndex - _bytesPerPixel]
            : (byte) 0;
    }

    private byte Predict(byte firstByte, byte secondByte, byte thirdByte)
    {
        var intermediateResult = firstByte + secondByte - thirdByte;

        var firstIntermediateResult = Math.Abs(intermediateResult - firstByte);
        var secondIntermediateResult = Math.Abs(intermediateResult - secondByte);
        var thirdIntermediateResult = Math.Abs(intermediateResult - thirdByte);

        if (firstIntermediateResult <= secondIntermediateResult && firstIntermediateResult <= thirdIntermediateResult)
        {
            return firstByte;
        }

        return secondIntermediateResult <= thirdIntermediateResult ? secondByte : thirdByte;
    }
}