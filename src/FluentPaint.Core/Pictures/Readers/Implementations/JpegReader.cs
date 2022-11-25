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
    
    private void ReadStartOfScanSection(IReadOnlyList<byte> section)
    {
        for (var i = 0; i < 3; i++)
        {
            _dcAcCoefficients.Add(new CoefficientsTable
            {
                DcCoefficientsTable = section[2 + i * 2] >> 4,
                AcCoefficientsTable = section[2 + i * 2] & 15
            });
        }
    }
    
    private void ReadFileInformation(byte[] section)
    {
        var bits = new List<int>();

        foreach (var element in section)
        {
            var currentByte = element;

            for (var i = 0; i < 8; i++)
            {
                bits.Add((currentByte & 128) >> 7 == 1 ? 1 : 0);

                currentByte = (byte) (currentByte << 1);
            }
        }

        // for (int currentMatrix = 0; currentMatrix < ; currentMatrix++)
        // {
        //     for (var i = 0; i < _thinning[0].Item1 + _thinning[0].Item2; i++)
        //     {
        //         bits = ReadTable(bits, _DcAcCoefficients[0]);
        //     }
        //
        //     bits = ReadTable(bits, _DcAcCoefficients[1]);
        //     bits = ReadTable(bits, _DcAcCoefficients[2]);
        // }

        var allTablesNumber = _width * _height / 256 * 6;

        var yTablesNumber = allTablesNumber * 2 / 3;
        var cbTablesNumber = allTablesNumber * 1 / 6;
        var crTablesNumber = allTablesNumber * 1 / 6;

        for (int currentMatrix = 0; currentMatrix < yTablesNumber; currentMatrix++)
        {
            bits = ReadTable(bits, _dcAcCoefficients[0]);
            // for (var i = 0; i < _thinning[0].Item1 + _thinning[0].Item2; i++)
            // {
            //     bits = ReadTable(bits, _DcAcCoefficients[0]);
            // }
        }

        for (int i = 0; i < cbTablesNumber; i++)
        {
            bits = ReadTable(bits, _dcAcCoefficients[1]);
        }

        for (int i = 0; i < crTablesNumber; i++)
        {
            bits = ReadTable(bits, _dcAcCoefficients[2]);
        }
    }
    
    private List<int> ReadTable(List<int> bits, CoefficientsTable coefficientsTable)
    {
        _isTopSideMatrix = true;
        var bitsPointer = 0;
        var coefficientTable = new int[8, 8];

        var huffmanTree = _huffmanTrees[coefficientsTable.DcCoefficientsTable + coefficientsTable.AcCoefficientsTable];
        var currentNode = huffmanTree.Root;

        (bitsPointer, currentNode) = GetNodeCode(currentNode, bitsPointer, bits);

        if (currentNode.Code == 0)
        {
            coefficientTable[0, 0] = currentNode.Code;
        }
        else
        {
            var currentCoefficientInBinarySystem = new List<int>();

            for (var bit = 0; bit < currentNode.Code; bit++)
            {
                currentCoefficientInBinarySystem.Add(bits[bitsPointer]);
                bitsPointer += 1;
            }

            var currentCoefficient = 0;

            for (var i = 0; i < currentCoefficientInBinarySystem.Count; i++)
            {
                currentCoefficient += (int) (currentCoefficientInBinarySystem[i] *
                                             Math.Pow(2, currentCoefficientInBinarySystem.Count - i - 1));
            }

            var currentCoefficientLength = currentCoefficientInBinarySystem.Count;

            if (currentCoefficientInBinarySystem[0] != 1)
            {
                currentCoefficient = currentCoefficient - (int) Math.Pow(2, currentCoefficientLength) + 1;
            }

            coefficientTable[0, 0] = currentCoefficient;
        }

        var currentPoint = new Point
        {
            Row = 0,
            Column = 0,
            DiagonalVector = false
        };

        huffmanTree = _huffmanTrees[coefficientsTable.DcCoefficientsTable + coefficientsTable.AcCoefficientsTable + 1];
        currentNode = huffmanTree.Root;

        currentPoint = GetNewPoint(currentPoint);

        while (currentPoint.Row < 7 && currentPoint.Column < 7)
        {
            (bitsPointer, currentNode) = GetNodeCode(currentNode, bitsPointer, bits);

            var currentCoefficient = 0;

            if (currentNode.Code == 0)
            {
                break;
            }
            else
            {
                var lengthOfCoefficient = currentNode.Code & 15;
                var lengthOfZeros = currentNode.Code >> 4;

                var currentCoefficientInBinarySystem = new List<int>();

                for (var bit = 0; bit < lengthOfCoefficient; bit++)
                {
                    currentCoefficientInBinarySystem.Add(bits[bitsPointer]);
                    bitsPointer += 1;
                }

                for (var i = 0; i < currentCoefficientInBinarySystem.Count; i++)
                {
                    currentCoefficient += (int) (currentCoefficientInBinarySystem[i] *
                                                 Math.Pow(2, currentCoefficientInBinarySystem.Count - i - 1));
                }

                for (var i = 0; i < lengthOfZeros; i++)
                {
                    coefficientTable[currentPoint.Row, currentPoint.Column] = 0;

                    currentPoint = GetNewPoint(currentPoint);
                }

                var currentCoefficientLength = currentCoefficientInBinarySystem.Count;

                if (currentCoefficientInBinarySystem[0] != 1)
                {
                    currentCoefficient = currentCoefficient - (int) Math.Pow(2, currentCoefficientLength) + 1;
                }

                coefficientTable[currentPoint.Row, currentPoint.Column] = currentCoefficient;

                currentPoint = GetNewPoint(currentPoint);
            }

            currentNode = huffmanTree.Root;
        }

        _dcAcCoefficientsTables.Add(coefficientTable);

        return bits.GetRange(bitsPointer, bits.Count - bitsPointer);
    }
    
    private (int, Node) GetNodeCode(Node node, int bitsPointer, List<int> bits)
    {
        while (node.Left is not null || node.Right is not null)
        {
            node = bits[bitsPointer] == 1 ? node.Right : node.Left;

            bitsPointer += 1;
        }

        return (bitsPointer, node);
    }
    
    private void CorrectDcCoefficients()
    {
        for (var i = 1; i < _thinning[0].Item1 + _thinning[0].Item2; i++)
        {
            _dcAcCoefficientsTables[i][0, 0] += _dcAcCoefficientsTables[i - 1][0, 0];
        }
    }
    
    private void QuantizeCoefficientTables()
    {
        var firstChannelTablesNumber = _thinning[0].Item1 + _thinning[0].Item2;
        var secondChannelTablesNumber = _thinning[1].Item1;
        var thirdChannelTablesNumber = _thinning[2].Item1;

        var tableIndex = 0;

        for (; tableIndex < firstChannelTablesNumber; tableIndex++)
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    _dcAcCoefficientsTables[tableIndex][i, j] *= _quantizationTables[_quantMapping[0]][i, j];
                }
            }
        }

        var cbTableIndex = tableIndex + secondChannelTablesNumber;

        for (; tableIndex < cbTableIndex; tableIndex++)
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    _dcAcCoefficientsTables[tableIndex][i, j] *= _quantizationTables[_quantMapping[1]][i, j];
                }
            }
        }

        var crTableIndex = tableIndex + thirdChannelTablesNumber;

        for (; tableIndex < crTableIndex; tableIndex++)
        {
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    _dcAcCoefficientsTables[tableIndex][i, j] *= _quantizationTables[_quantMapping[2]][i, j];
                }
            }
        }
    }
}