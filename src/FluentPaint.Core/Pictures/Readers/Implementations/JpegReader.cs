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

        for (var currentMatrix = 0; currentMatrix < _height * _width / 64; currentMatrix += 6)
        {
            for (var i = 0; i < _thinning[0].Item1 + _thinning[0].Item2; i++)
            {
                bits = ReadTable(bits, _dcAcCoefficients[0]);
            }
            
            bits = ReadTable(bits, _dcAcCoefficients[1]);
            
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
    
    private void ProcessCosineTransform()
    {
        foreach (var currentMatrix in _dcAcCoefficientsTables)
        {
            var matrix = new int[8, 8];

            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var currentValue = 0.0;

                    for (var u = 0; u < 8; u++)
                    {
                        for (var v = 0; v < 8; v++)
                        {
                            var firstCoefficient = 1.0;
                            var secondCoefficient = 1.0;

                            if (u == 0)
                            {
                                firstCoefficient = 1 / Math.Sqrt(2);
                            }

                            if (v == 0)
                            {
                                secondCoefficient = 1 / Math.Sqrt(2);
                            }

                            currentValue += firstCoefficient * secondCoefficient * currentMatrix[v, u] *
                                            Math.Cos((2 * j + 1) * u * Math.PI / 16) *
                                            Math.Cos((2 * i + 1) * v * Math.PI / 16);
                        }
                    }
                    
                    matrix[i, j] = Math.Min(Math.Max(0, (int) (currentValue / 4) + 128), 255);
                }
            }

            _yCbCrMatrices.Add(matrix);
        }
    }
    
    private SKBitmap ConvertYCbCrToRgb()
    {
        var bitmap = new SKBitmap(_width, _height);
        
        var cbTablePointer = 0;
        var crTablePointer = 0;

        var yTableRow = 0;
        var yTableColumn = 0;
        var cbTableRow = 0;
        var cbTableColumn = 0;
        var crTableRow = 0;
        var crTableColumn = 0;
        
        var mcuRow = 0;
        var mcuColumn = 0;

        var brightnessTableNumber = _width * _height / 64;
        var cbTablesNumber = brightnessTableNumber / 4;
        // var crTablesNumber = brightnessTableNumber / 2;

        for (var brightnessTable = 0; brightnessTable < brightnessTableNumber; brightnessTable++)
        {
            if (brightnessTable != 0 && brightnessTable % 4 == 0)
            {
                cbTablePointer += 1;
                crTablePointer += 1;
            }
            
            if (mcuColumn == _width / 8)
            {
                mcuColumn = 0;
                mcuRow += 1;
            }

            // if (cbTableRow == 4 && cbTableColumn == 8)
            // {
            //     cbTableRow = 4;
            //     cbTableColumn = 0;
            // }
            //
            // if (crTableRow == 4 && crTableColumn == 8)
            // {
            //     crTableRow = 4;
            //     crTableColumn = 0;
            // }

            var currentYBlockRow = 0;
            var currentYBlockColumn = 0;

            for (var currentYBlock = 0; currentYBlock < 16; currentYBlock++)
            {
                for (var i = 0; i < 4; i++)
                {
                    bitmap.SetPixel(mcuRow * 8 + currentYBlockRow * 2 + yTableRow , mcuColumn * 8  + currentYBlockColumn * 2 + yTableColumn,
                        ConvertPixelYCbCrToRgb(_yCbCrMatrices[brightnessTable][yTableRow + currentYBlockRow * 2, yTableColumn + currentYBlockColumn * 2],
                            _yCbCrMatrices[brightnessTableNumber + cbTablePointer][cbTableRow, cbTableColumn],
                            _yCbCrMatrices[brightnessTableNumber + cbTablesNumber + crTablePointer][crTableRow, crTableColumn]));

                    switch (yTableRow)
                    {
                        case 0 when yTableColumn == 0:
                            yTableColumn = 1;
                            break;
                        case 0 when yTableColumn == 1:
                            yTableRow = 1;
                            yTableColumn = 0;
                            break;
                        case 1 when yTableColumn == 0:
                            yTableColumn = 1;
                            break;
                        case 1 when yTableColumn == 1:
                            yTableRow = 0;
                            yTableColumn = 0;
                            break;
                    }
                }
                
                currentYBlockColumn += 1;

                if (currentYBlockColumn == 4)
                {
                    currentYBlockColumn = 0;
                    currentYBlockRow += 1;
                    
                    if (brightnessTable % 4 == 0)
                    {
                        if (cbTableRow == 3)
                        {
                            cbTableRow = 0;
                            cbTableColumn = 4;
                        }
                        else
                        {
                            cbTableColumn = 0;
                            cbTableRow += 1;
                        }
                        
                        if (crTableRow == 3)
                        {
                            crTableRow = 0;
                            crTableColumn = 4;
                        }
                        else
                        {
                            crTableColumn = 0;
                            crTableRow += 1;
                        }
                    }
                    else if (brightnessTable % 4 == 1)
                    {
                        if (cbTableRow == 3)
                        {
                            cbTableRow = 4;
                            cbTableColumn = 0;
                        }
                        else
                        {
                            cbTableColumn = 4;
                            cbTableRow += 1;
                        }
                        
                        if (crTableRow == 3)
                        {
                            crTableRow = 4;
                            crTableColumn = 0;
                        }
                        else
                        {
                            crTableColumn = 4;
                            crTableRow += 1;
                        }
                    }
                    else if (brightnessTable % 4 == 2)
                    {
                        if (cbTableRow == 7)
                        {
                            cbTableRow = 4;
                            cbTableColumn = 4;
                        }
                        else
                        {
                            cbTableColumn = 0;
                            cbTableRow += 1;
                        }
                        
                        if (crTableRow == 7)
                        {
                            crTableRow = 4;
                            crTableColumn = 4;
                        }
                        else
                        {
                            crTableColumn = 0;
                            crTableRow += 1;
                        }
                    }
                    else if (brightnessTable % 4 == 3)
                    {
                        if (cbTableColumn == 7)
                        {
                            cbTableColumn = 4;
                            cbTableRow += 1;
                        }

                        if (crTableColumn == 7)
                        {
                            crTableColumn = 4;
                            crTableRow += 1;
                        }
                    }
                }
                else
                {
                    cbTableColumn += 1;
                    crTableColumn += 1;
                }
            }

            mcuColumn += 1;
        }

        // var blockRow = 0;
        // var blockColumn = 0;
        //
        // var yMatrixCounter = 0;
        // var yMatrixPointer = 0;
        // var cbMatrixPointer = 0;
        // var crMatrixPointer = 0;
        //
        // for (var brightnessTable = 0; brightnessTable < _width * _height / 64; brightnessTable++)
        // {
        //     if (yMatrixCounter == 4)
        //     {
        //         yMatrixPointer += 2;
        //         yMatrixCounter = 0;
        //         cbMatrixPointer += 1;
        //         crMatrixPointer += 1;
        //     }
        //
        //     if (blockColumn == _width / 8)
        //     {
        //         blockColumn = 0;
        //         blockRow += 1;
        //     }
        //
        //     for (var i = 0; i < 8; i++)
        //     {
        //         for (var j = 0; j < 8; j++)
        //         {
        //             // var yValue = _yCbCrMatrices[yMatrixPointer][i, j];
        //             // var cbValue = _yCbCrMatrices[yMatrixPointer - yMatrixCounter + 4][i, j];
        //             // var crValue = _yCbCrMatrices[yMatrixPointer - yMatrixCounter + 5][i, j];
        //             
        //             var allTablesNumber = _width * _height / 256 * 6;
        //
        //             var yTablesNumber = allTablesNumber * 2 / 3;
        //             var cbTablesNumber = allTablesNumber * 1 / 6;
        //
        //             var yValue = _yCbCrMatrices[brightnessTable][i, j];
        //             var cbValue = _yCbCrMatrices[yTablesNumber + cbMatrixPointer][i, j];
        //             var crValue = _yCbCrMatrices[yTablesNumber + cbTablesNumber + crMatrixPointer][i, j];
        //
        //             var redComponent = yValue + 1.402 * (crValue - 128);
        //             var greenComponent = yValue - 0.34414 * (cbValue - 128) - 0.71414 * (crValue - 128);
        //             var blueComponent = yValue + 1.772 * (cbValue - 128);
        //
        //             redComponent = Math.Min(Math.Max(0, redComponent), 255);
        //             greenComponent = Math.Min(Math.Max(0, greenComponent), 255);
        //             blueComponent = Math.Min(Math.Max(0, blueComponent), 255);
        //
        //
        //             var pixel =
        //                 new SKColor(
        //                     (byte) redComponent,
        //                     (byte) greenComponent,
        //                     (byte) blueComponent
        //                 );
        //
        //             bitmap.SetPixel(i + 8 * blockRow, j + 8 * blockColumn, pixel);
        //         }
        //     }
        //
        //     yMatrixPointer += 1;
        //     yMatrixCounter += 1;
        //     blockColumn += 1;
        // }

        var finalBitmap = new SKBitmap(_width, _height);
        
        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                finalBitmap.SetPixel(i, j, bitmap.GetPixel(j , i));
            }
        }
        
        return finalBitmap;
    }

    private SKColor ConvertPixelYCbCrToRgb(int yValue, int cbValue, int crValue)
    {
        var redComponent = yValue + 1.402 * (crValue - 128);
        var greenComponent = yValue - 0.34414 * (cbValue - 128) - 0.71414 * (crValue - 128);
        var blueComponent = yValue + 1.772 * (cbValue - 128);

        redComponent = Math.Min(Math.Max(0, redComponent), 255);
        greenComponent = Math.Min(Math.Max(0, greenComponent), 255);
        blueComponent = Math.Min(Math.Max(0, blueComponent), 255);

        return new SKColor((byte) redComponent, (byte) greenComponent, (byte) blueComponent);
    }
}