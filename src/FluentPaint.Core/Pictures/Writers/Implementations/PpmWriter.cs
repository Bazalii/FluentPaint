using FluentPaint.Core.Enums;
using FluentPaint.Core.Pictures.HeaderWriters.Implementations;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.Writers.Implementations;

/// <summary>
/// Writer to .ppm files
/// </summary>
public class PpmWriter : IPictureWriter
{
    private readonly PnmHeaderWriter _pnmHeaderWriter = new();
    
    public void WriteImageData(FileStream fileStream, FluentBitmap bitmap, PictureType type)
    {
        _pnmHeaderWriter.Write(fileStream, bitmap, type);
        
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                
                fileStream.WriteByte(pixel.Red);
                fileStream.WriteByte(pixel.Green);
                fileStream.WriteByte(pixel.Blue);
            }
        }

        fileStream.Close();
    }
}