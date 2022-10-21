using SkiaSharp;

namespace FluentPaint.Core.Сonverters.Implementations;

public class CmyConverter : IConverter
{
    public SKBitmap FromRgb(SKBitmap bitmap)
    {
        var newBitmap = new SKBitmap();
        
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                double cyan = 1 - pixel.Red / 255;
                double magenta = 1 - pixel.Green / 255;
                double yellow = 1 - pixel.Blue / 255;
                newBitmap.SetPixel(x, y, new SKColor((byte)cyan, (byte)magenta, (byte)yellow));
            }
        }

        return newBitmap;
    }

    public SKBitmap ToRgb(SKBitmap bitmap)
    {
        var newBitmap = new SKBitmap();
        
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                var cyan = pixel.Red;
                var magenta = pixel.Green;
                var yellow = pixel.Blue;
                
                double red = 255 - 255 * cyan;
                double green = 255 - 255 * magenta;
                double blue = 255 - 255 * yellow;
                newBitmap.SetPixel(x, y, new SKColor((byte)red, (byte)green, (byte)blue));
            }
        }

        return newBitmap;
    }
}