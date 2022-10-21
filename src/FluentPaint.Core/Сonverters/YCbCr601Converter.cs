using SkiaSharp;

namespace FluentPaint.Core.Сonverters;

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

                var Y = A * red + B * green + C * blue;
                var Cb = (blue - Y) / D;
                var Cr = (red - Y) / E;

                convertedBitmap.SetPixel(x, y, new SKColor((byte) Y, (byte) Cb,(byte) Cr));
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

                var Y = pixel.Red;
                var Cb = pixel.Green;
                var Cr = pixel.Blue;

                var red = Y + E * Cr;
                var green = Y - (A * E / B) * Cr - (C * D / B) * Cb;
                var blue = Y + D * Cb;
                
                convertedBitmap.SetPixel(x, y, new SKColor((byte) red,(byte) green,(byte) blue));
            }
        }

        return convertedBitmap;
    }
}