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

                var red = pixel.Red / 255f;
                var blue = pixel.Blue / 255f;
                var green = pixel.Green / 255f;

                /*var luminance = A * red + B * green + C * blue;
                var blueComponent = (blue - luminance) / D;
                var redComponent = (red - luminance) / E;*/

                var luminance = 16 + 65.481f * red + 128.553f * green + 24.996f * blue;
                var blueComponent = 128 - 37.797f * red - 74.203 * green + 112.0f * blue;
                var redComponent = 128 + 112.0f * red - 93.786f * green - 18.214f * blue;

                /*blueComponent = blueComponent switch
                {
                    > 240 => 240,
                    < 16 => 16,
                    _ => blueComponent
                };
                
                redComponent = redComponent switch
                {
                    > 240 => 240,
                    < 16 => 16,
                    _ => redComponent
                };*/

                convertedBitmap.SetPixel(x, y,
                    new SKColor((byte) luminance, (byte) blueComponent, (byte) redComponent));
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

                var luminance = pixel.Red / 256f;
                var blueComponent = pixel.Green / 256f;
                var redComponent = pixel.Blue / 256f;
                
                
                /*blueComponent = blueComponent switch
                {
                    > 240 / 256f => 240 / 256f,
                    < 16 / 256f => 16 / 256f,
                    _ => blueComponent
                };
                
                redComponent = redComponent switch
                {
                    > 240 / 256f=> 240/ 256f,
                    < 16/ 256f => 16/ 256f,
                    _ => redComponent
                };*/

                var red = 298.082f * luminance + 408.583f * redComponent - 222.921;
                var green = 298.082f * luminance - 100.291f * blueComponent - 208.120f * redComponent + 135.576;
                var blue = 298.082f * luminance + 516.412f * blueComponent - 276.836;

                /*var red = (255 / 219) * (luminance - 16) + 255f / 224 * 1.402f * (redComponent - 128);

                var green = (255 / 219) * (luminance - 16) - (255f / 214) * 1.772f * (0.114f / 0.587f) *
                                                           (blueComponent - 128)
                                                           - (255f / 214) * 1.402f * (0.299f / 0.587) *
                                                           (redComponent - 128);

                var blue = (255 / 219) * (luminance - 16) + (255f / 214) * 1.772f * (blueComponent - 128);*/

                convertedBitmap.SetPixel(x, y, new SKColor((byte) red, (byte) green, (byte) blue));
            }
        }

        return convertedBitmap;
    }
}