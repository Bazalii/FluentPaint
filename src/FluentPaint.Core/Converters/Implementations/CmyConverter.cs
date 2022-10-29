using SkiaSharp;

namespace FluentPaint.Core.Converters.Implementations;

/// <summary>
/// Provides methods to convert pictures from RGB to CMY, and in the opposite direction.
/// </summary>
public class CmyConverter : IConverter
{
    /// <summary>
    /// Convert picture from RGB to CMY.
    /// </summary>
    /// <param name="bitmap"><see cref="SKBitmap"/> Bitmap containing all pixels of the picture. </param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    public SKBitmap FromRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                double cyan = 255 - pixel.Red;
                double magenta = 255 - pixel.Green;
                double yellow = 255 - pixel.Blue;

                convertedBitmap.SetPixel(x, y, new SKColor((byte) cyan, (byte) magenta, (byte) yellow));
            }
        }

        return convertedBitmap;
    }

    /// <summary>
    /// Convert picture from CMY to RGB.
    /// </summary>
    /// <param name="bitmap"> <see cref="SKBitmap"/> Bitmap containing all pixels of the picture. </param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    public SKBitmap ToRgb(SKBitmap bitmap)
    {
        var convertedBitmap = new SKBitmap(bitmap.Width, bitmap.Height);

        for (var y = 0; y < bitmap.Height; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                var pixel = bitmap.GetPixel(x, y);

                var cyan = pixel.Red;
                var magenta = pixel.Green;
                var yellow = pixel.Blue;

                double red = 255 - cyan;
                double green = 255 - magenta;
                double blue = 255 - yellow;

                convertedBitmap.SetPixel(x, y, new SKColor((byte) red, (byte) green, (byte) blue));
            }
        }

        return convertedBitmap;
    }
}