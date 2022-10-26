using SkiaSharp;

namespace FluentPaint.Core.Converters.Implementations;

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
                
                float chrominanceGreen = pixel.Red - pixel.Blue;
                var chrominanceOrange = pixel.Green - pixel.Blue + chrominanceGreen / 2;
                var luma = pixel.Blue + chrominanceGreen / 2 + chrominanceOrange / 2;
                
                newBitmap.SetPixel(x, y, new SKColor((byte)luma, (byte)chrominanceGreen, (byte)chrominanceOrange));
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
                float chrominanceOrange = pixel.Green;
                float chrominanceGreen = pixel.Blue;
                
                var green = chrominanceGreen + luma - chrominanceGreen / 2;
                var blue = luma - chrominanceGreen / 2 - chrominanceOrange / 2;
                var red = blue + chrominanceOrange;
                
                newBitmap.SetPixel(x, y, new SKColor((byte)red, (byte)green, (byte)blue));
            }
        }

        return newBitmap;
    }
}