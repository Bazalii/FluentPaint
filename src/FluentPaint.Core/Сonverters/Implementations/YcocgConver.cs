using SkiaSharp;

namespace FluentPaint.Core.Сonverters.Implementations;

public class YcocgConver : IConverter
{
    public SKBitmap FromRgb(SKBitmap bitmap)
    {
        var newBitmap = new SKBitmap();
        
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);
                float co = pixel.Red - pixel.Blue;
                float cg = pixel.Green - pixel.Blue + co / 2;
                float luma = pixel.Blue + co / 2 + cg / 2;
                newBitmap.SetPixel(x, y, new SKColor((byte)luma, (byte)co, (byte)cg));
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
                float luma = pixel.Red;
                float co = pixel.Green;
                float cg = pixel.Blue;
                
                float green = cg + luma - cg / 2;
                float blue = luma - cg / 2 - co / 2;
                float red = blue + co;
                newBitmap.SetPixel(x, y, new SKColor((byte)red, (byte)green, (byte)blue));
            }
        }

        return newBitmap;
    }
}