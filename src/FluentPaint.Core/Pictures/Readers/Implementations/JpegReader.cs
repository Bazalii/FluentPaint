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
}