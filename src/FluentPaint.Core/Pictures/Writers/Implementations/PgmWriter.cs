using System.Text;
using FluentPaint.Core.Enums;
using FluentPaint.Core.Pictures.HeaderWriters.Implementations;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.Writers.Implementations;

/// <summary>
/// Writer to .pgm files
/// </summary>
public class PgmWriter : IPictureWriter
{
    private readonly PnmHeaderWriter _pnmHeaderWriter = new();
    
    public void WriteImageData(FileStream fileStream, SKBitmap bitmap, PictureType type)
    {
        _pnmHeaderWriter.Write(fileStream, bitmap, type);  
        
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var luma = (int) (pixel.Red * 0.3 + pixel.Green * 0.59 + pixel.Blue * 0.11);

                fileStream.WriteByte((byte) luma);
            }
        }

        fileStream.Close();
    }
}