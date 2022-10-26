using SkiaSharp;

namespace FluentPaint.Core.Converters.Implementations;

public class YCbCr601Converter : IConverter
{
    
    private const float A = 0.299f;
    private const float B = 0.587f;
    private const float C = 0.114f;
    private const float D = 1.772f;
    private const float E = 1.402f;
    
    public SKBitmap FromRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var red = pixel.Red;
                var blue = pixel.Blue;
                var green = pixel.Green;

                var luminance = A * red + B * green + C * blue;
                var blueComponent = (blue - luminance) / D;
                var redComponent = (red - luminance) / E;

                convertedBitmap.SetPixel(x, y, new SKColor((byte) luminance, (byte) blueComponent,(byte) redComponent));
            }
        }

        return convertedBitmap;
    }

    public SKBitmap ToRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var luminance = pixel.Red;
                var blueComponent = pixel.Green;
                var redComponent = pixel.Blue;

                var red = luminance + E * redComponent;
                var green = luminance - (A * E / B) * redComponent - (C * D / B) * blueComponent;
                var blue = luminance + D * blueComponent;
                
                convertedBitmap.SetPixel(x, y, new SKColor((byte) red,(byte) green,(byte) blue));
            }
        }

        return convertedBitmap;
    }
}