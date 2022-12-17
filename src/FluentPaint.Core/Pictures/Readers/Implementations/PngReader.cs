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
    private float _gamma = 1.0f;

    private byte[,]? _palette;

    private List<byte> _encodedImage = new();
    private List<byte> _decodedImage = new();

    public FluentBitmap ReadImageData(FileStream fileStream)
    {
        var buffer = new byte[8];
        var pointer = 0;

        pointer += fileStream.Read(buffer);

        buffer = new byte[4];

        while (pointer < fileStream.Length)
        {
            pointer += fileStream.Read(buffer);

            Array.Reverse(buffer);
            var currentSectionLength = BitConverter.ToInt32(buffer, 0);

            pointer += fileStream.Read(buffer);
            var currentSection = new ASCIIEncoding().GetString(buffer);

            var sectionBuffer = new byte[currentSectionLength];
            pointer += fileStream.Read(sectionBuffer);

            var cyclicRedundancyCodeBuffer = new byte[4];
            pointer += fileStream.Read(cyclicRedundancyCodeBuffer);

            switch (currentSection)
            {
                case "IHDR":
                    ReadHeader(sectionBuffer);
                    break;
                case "IDAT":
                    _encodedImage.AddRange(sectionBuffer);
                    break;
                case "gAMA":
                    Array.Reverse(sectionBuffer);
                    _gamma = (float) BitConverter.ToInt32(sectionBuffer, 0) / 100000;
                    break;
                case "PLTE":
                    ReadPalette(sectionBuffer);
                    break;
                case "IEND":
                    _bytesPerPixel = _colorType switch
                    {
                        0 => 1,
                        2 => 3,
                        3 => 1,
                        _ => throw new ArgumentException("Color type is not supported!")
                    };

                    _stride = _width * _bytesPerPixel;

                    var outputStream = new MemoryStream();
                    var compressedStream = new MemoryStream(_encodedImage.ToArray());
                    var inputStream = new InflaterInputStream(compressedStream);
                    inputStream.CopyTo(outputStream);

                    GetImage(outputStream.ToArray());

                    return _colorType switch
                    {
                        0 => WriteGrayScaleImage(),
                        2 => WriteTrueColorImage(),
                        3 => WriteIndexedColorImage(),
                        _ => throw new ArgumentException("Color type is not supported!")
                    };
            }
        }

        throw new Exception("Something went wrong with png file!");
    }

    private void ReadHeader(byte[] section)
    {
        _width = BitConverter.ToInt32(section[..4].Reverse().ToArray(), 0);
        _height = BitConverter.ToInt32(section[4..8].Reverse().ToArray(), 0);
        _bitDepth = BitConverter.ToInt32(new byte[] { section[8], 0, 0, 0 }, 0);
        _colorType = BitConverter.ToInt32(new byte[] { section[9], 0, 0, 0 }, 0);
        var compressionMethod = BitConverter.ToInt32(new byte[] { section[10], 0, 0, 0 }, 0);
        var filterMethod = BitConverter.ToInt32(new byte[] { section[11], 0, 0, 0 }, 0);
        var interlaceMethod = BitConverter.ToInt32(new byte[] { section[12], 0, 0, 0 }, 0);

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

    private void ReadPalette(byte[] section)
    {
        _palette = new byte[(int) Math.Pow(2, _bitDepth), 3];

        var rowPointer = 0;

        for (var i = 0; i < section.Length; i += 3)
        {
            _palette[rowPointer, 0] = section[i];
            _palette[rowPointer, 1] = section[i + 1];
            _palette[rowPointer, 2] = section[i + 2];

            rowPointer += 1;
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
                    1 => filteredByte + ReconstructionA(i, j),
                    2 => filteredByte + ReconstructionB(i, j),
                    3 => filteredByte + (ReconstructionA(i, j) + ReconstructionB(i, j)) / 2,
                    4 => filteredByte + Predict(ReconstructionA(i, j), ReconstructionB(i, j), ReconstructionC(i, j)),
                    _ => throw new Exception("Filter is not supported!")
                };

                _decodedImage.Add((byte) (reconstructedPixel & 0xff));
            }
        }
    }

    private byte ReconstructionA(int reconstructedIndex, int alongScanlineIndex)
    {
        return alongScanlineIndex >= _bytesPerPixel
            ? _decodedImage[reconstructedIndex * _stride + alongScanlineIndex - _bytesPerPixel]
            : (byte) 0;
    }

    private byte ReconstructionB(int reconstructedIndex, int alongScanlineIndex)
    {
        return reconstructedIndex > 0
            ? _decodedImage[(reconstructedIndex - 1) * _stride + alongScanlineIndex]
            : (byte) 0;
    }

    private byte ReconstructionC(int reconstructedIndex, int alongScanlineIndex)
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

    private FluentBitmap WriteGrayScaleImage()
    {
        var bitmap = new FluentBitmap(_width, _height);

        var imagePointer = 0;

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                var color = _decodedImage[imagePointer];

                bitmap.SetPixel(x, y, new SKColor(color, color, color));

                imagePointer += 1;
            }
        }

        bitmap.Gamma = _gamma;

        return bitmap;
    }

    private FluentBitmap WriteTrueColorImage()
    {
        var bitmap = new FluentBitmap(_width, _height);

        var imagePointer = 0;

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                bitmap.SetPixel(x, y, new SKColor(
                    _decodedImage[imagePointer],
                    _decodedImage[imagePointer + 1],
                    _decodedImage[imagePointer + 2])
                );

                imagePointer += 3;
            }
        }

        bitmap.Gamma = _gamma;

        return bitmap;
    }

    private FluentBitmap WriteIndexedColorImage()
    {
        var bitmap = new FluentBitmap(_width, _height);

        var imagePointer = 0;

        for (var y = 0; y < _height; y++)
        {
            for (var x = 0; x < _width; x++)
            {
                var paletteRowIndex = _decodedImage[imagePointer];

                bitmap.SetPixel(x, y, new SKColor(
                    _palette[paletteRowIndex, 0],
                    _palette[paletteRowIndex, 1],
                    _palette[paletteRowIndex, 2])
                );

                imagePointer += 1;
            }
        }

        bitmap.Gamma = _gamma;

        return bitmap;
    }
}