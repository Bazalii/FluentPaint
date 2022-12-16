using FluentPaint.Core.Pictures.HeaderReaders.Implementations;
using SkiaSharp;

namespace FluentPaint.Core.Pictures.Readers.Implementations;

/// <summary>
/// Reader for .ppm files.
/// </summary>
public class PpmReader : IPictureReader
{
    private readonly PnmHeaderReader _pnmHeaderReader = new();

    public FluentBitmap ReadImageData(FileStream fileStream)
    {
        var pictureSize = _pnmHeaderReader.Read(fileStream);

        var bitmap = new FluentBitmap(pictureSize.Width, pictureSize.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var red = (byte) fileStream.ReadByte();
                var green = (byte) fileStream.ReadByte();
                var blue = (byte) fileStream.ReadByte();

                bitmap.SetPixel(x, y, new SKColor(red, green, blue));
            }
        }

        return bitmap;
    }
}