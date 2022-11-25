using FluentPaint.Core.Huffman;
using FluentPaint.Core.Pictures.Readers.Models;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.Readers.Implementations;

public class JpegReader : IPictureReader
{
    private List<byte[,]> _quantizationTables = new();
    private List<int[,]> _dcAcCoefficientsTables = new();
    private int _width;
    private int _height;
    private List<(int, int)> _thinning = new();
    private List<byte> _quantMapping = new();
    private List<HuffmanTree> _huffmanTrees = new();
    private List<CoefficientsTable> _dcAcCoefficients = new();
    private bool _isTopSideMatrix = true;
    private List<int[,]> _yCbCrMatrices = new();

    public SKBitmap ReadImageData(FileStream fileStream)
    {
        var pointer = 0;
        var markerBuffer = new byte[2];

        var bitmap = new SKBitmap();

        while (pointer < fileStream.Length)
        {
            fileStream.Read(markerBuffer);

            var currentSection = Convert.ToHexString(markerBuffer);

            if (currentSection == "FFD9")
            {
                pointer += 2;
            }
            else if (currentSection == "FFD8")
            {
                pointer += 2;
            }
            else
            {
                fileStream.Read(markerBuffer);

                var currentSectionLength = int.Parse(Convert.ToHexString(markerBuffer),
                    System.Globalization.NumberStyles.HexNumber);

                var sectionBuffer = new byte[currentSectionLength - 2];

                fileStream.Read(sectionBuffer);

                pointer += 2;

                switch (currentSection)
                {
                    case "FFC4":
                        ReadHuffmanTree(sectionBuffer);
                        pointer += currentSectionLength;
                        break;
                    case "FFDB":
                        ReadQuantizationTable(sectionBuffer);
                        pointer += currentSectionLength;
                        break;
                    case "FFC0":
                        CalculateBaselineDct(sectionBuffer);
                        pointer += currentSectionLength;
                        break;
                    case "FFDA":
                        ReadStartOfScanSection(sectionBuffer);
                        pointer += currentSectionLength;

                        sectionBuffer = new byte[fileStream.Length - pointer - 4];
                        fileStream.Read(sectionBuffer);

                        ReadFileInformation(sectionBuffer);
                        CorrectDcCoefficients();
                        QuantizeCoefficientTables();
                        ProcessCosineTransform();
                        bitmap = ConvertYCbCrToRgb();
                        break;
                }
            }
        }

        return bitmap;
    }
    
    private void ReadHuffmanTree(byte[] section)
    {
        var offset = 0;
        var header = section[offset];
        offset += 1;

        var numbersOfSymbols = section[offset..(offset + 16)];
        offset += 16;

        var symbolCodes = section[offset..];

        var huffmanTree = new HuffmanTree(header, numbersOfSymbols, symbolCodes);

        _huffmanTrees.Add(huffmanTree);
    }
    
    private void CalculateBaselineDct(byte[] section)
    {
        _height = int.Parse(Convert.ToHexString(section[1..3]), System.Globalization.NumberStyles.HexNumber);
        _width = int.Parse(Convert.ToHexString(section[3..5]), System.Globalization.NumberStyles.HexNumber);

        var components = section[5];

        for (var i = 0; i < components; i++)
        {
            _thinning.Add((section[7 + i * 3] >> 4, section[7 + i * 3] & 15));
            _quantMapping.Add(section[8 + i * 3]);
        }
    }
    
    private void ReadQuantizationTable(IReadOnlyList<byte> section)
    {
        var quantizationTable = new byte[8, 8];

        var currentPoint = new Point
        {
            Row = 0,
            Column = 0,
            DiagonalVector = false
        };

        for (var i = 1; i < 65; i++)
        {
            quantizationTable[currentPoint.Row, currentPoint.Column] = section[i];

            currentPoint = GetNewPoint(currentPoint);
        }

        _isTopSideMatrix = true;

        _quantizationTables.Add(quantizationTable);
    }
    
    private Point GetNewPoint(Point currentPoint)
    {
        var newPoint = new Point();

        if (_isTopSideMatrix)
        {
            if (currentPoint.Row == 0 && currentPoint.Column == 0)
            {
                newPoint.Row = 0;
                newPoint.Column = 1;

                return newPoint;
            }

            if (currentPoint.Row == 0 && currentPoint.Column == 1)
            {
                newPoint.Row = 1;
                newPoint.Column = 0;

                return newPoint;
            }

            if (currentPoint.Row == 0 && currentPoint.DiagonalVector)
            {
                newPoint.Column = currentPoint.Column + 1;
                newPoint.DiagonalVector = false;
            }
            else if (currentPoint.Column == 0 && !currentPoint.DiagonalVector)
            {
                newPoint.Row = currentPoint.Row + 1;
                newPoint.DiagonalVector = true;
            }
            else
            {
                if (currentPoint.DiagonalVector)
                {
                    newPoint.Row = currentPoint.Row - 1;
                    newPoint.Column = currentPoint.Column + 1;
                    newPoint.DiagonalVector = true;
                }
                else
                {
                    newPoint.Row = currentPoint.Row + 1;
                    newPoint.Column = currentPoint.Column - 1;
                }
            }

            if (newPoint.Row == 7 && newPoint.Column == 0)
            {
                _isTopSideMatrix = false;
            }
        }
        else
        {
            if (currentPoint.Row == 7 && currentPoint.Column == 6)
            {
                newPoint.Row = 7;
                newPoint.Column = 7;

                return newPoint;
            }

            if (currentPoint.Row == 7 && !currentPoint.DiagonalVector)
            {
                newPoint.Row = currentPoint.Row;
                newPoint.Column = currentPoint.Column + 1;
                newPoint.DiagonalVector = true;
            }
            else if (currentPoint.Column == 7 && currentPoint.DiagonalVector)
            {
                newPoint.Row = currentPoint.Row + 1;
                newPoint.Column = currentPoint.Column;
                newPoint.DiagonalVector = false;
            }
            else
            {
                if (currentPoint.DiagonalVector)
                {
                    newPoint.Row = currentPoint.Row - 1;
                    newPoint.Column = currentPoint.Column + 1;
                    newPoint.DiagonalVector = true;
                }
                else
                {
                    newPoint.Row = currentPoint.Row + 1;
                    newPoint.Column = currentPoint.Column - 1;
                }
            }
        }

        return newPoint;
    }
}