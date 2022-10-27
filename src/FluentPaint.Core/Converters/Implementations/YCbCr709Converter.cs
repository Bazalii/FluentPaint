using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using SkiaSharp;

namespace FluentPaint.Core.Converters.Implementations;

public class YCbCr709Converter : IConverter
{
    private const float A = 0.2126f;
    private const float B = 0.7152f;
    private const float C = 0.0752f;
    private const float D = 1.8556f;
    private const float E = 1.5748f;

    public SKBitmap FromRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var red = pixel.Red / 255f;
                var blue = pixel.Blue / 255f;
                var green = pixel.Green / 255f;

                Matrix<double> a = Matrix<double>.Build.Dense(3, 3);
                Vector<double> v = Vector<double>.Build.Dense(3);

                v[0] = red;
                v[1] = green;
                v[2] = blue;

                a[0, 0] = 0.2126;
                a[0, 1] = 0.7152;
                a[0, 2] = 0.0722;
                a[1, 0] = -0.1146;
                a[1, 1] = -0.3854;
                a[1, 2] = 0.5;
                a[2, 0] = 0.5;
                a[2, 1] = -0.4542;
                a[2, 2] = -0.0458;

                var result = a * v;

                var luminance = result[0];
                var blueComponent = result[1];
                var redComponent = result[2];

                convertedBitmap.SetPixel(x, y,
                    new SKColor((byte) (luminance * 255), (byte) (blueComponent * 255), (byte) (redComponent * 255)));
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

                var luminance = pixel.Red/ 255f;
                var blueComponent = pixel.Green/ 255f;
                var redComponent = pixel.Blue/ 255f;


                var a = Matrix<double>.Build.Dense(3, 3);
                var v = Vector<double>.Build.Dense(3);

                v[0] = luminance;
                v[1] = blueComponent;
                v[2] = redComponent;

                a[0, 0] = 1;
                a[0, 1] = 0;
                a[0, 2] = 1.5748;
                a[1, 0] = 1;
                a[1, 1] = -0.1873;
                a[1, 2] = -0.4681;
                a[2, 0] = 1;
                a[2, 1] = 1.8556;
                a[2, 2] = 0;

                var result = a * v;

                var red = result[0];
                var green = result[1];
                var blue = result[2];

                convertedBitmap.SetPixel(x, y, new SKColor((byte) (red * 255), (byte) (green * 255), (byte) (blue * 255)));
            }
        }

        return convertedBitmap;
    }
}