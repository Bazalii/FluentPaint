using SkiaSharp;

namespace FluentPaint.Core.Pictures.Readers.Implementations;

/// <summary>
/// Reader for .ppm files.
/// </summary>
public class PpmReader : IPictureReader
{
    public SKBitmap ReadImageData(FileStream fileStream, int width, int height)
    {
        var bitmap = new SKBitmap(width, height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var red = (byte)fileStream.ReadByte();
                var green = (byte)fileStream.ReadByte();
                var blue = (byte)fileStream.ReadByte();

                bitmap.SetPixel(x, y, new SKColor(red, green, blue));
            }
        }

        return bitmap;
    }
}