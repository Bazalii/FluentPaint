using SkiaSharp;

namespace FluentPaint.Core.Pictures.Writers.Implementations;

/// <summary>
/// Writer to .ppm files
/// </summary>
public class PpmWriter : IPictureWriter
{
    public void WriteImageData(FileStream fileStream, SKBitmap bitmap)
    {
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