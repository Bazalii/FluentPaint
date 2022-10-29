using SkiaSharp;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace FluentPaint.UI;

/// <summary>
/// Contains extension methods for <see cref="SKBitmap"/>.
/// </summary>
public static class SKBitmapExtensions
{
    /// <summary>
    /// Converts <see cref="SKBitmap"/> to <see cref="Bitmap"/>.
    /// </summary>
    /// <param name="bitmap"> <see cref="SKBitmap"/> to convert. </param>
    /// <returns>
    /// Converted <see cref="Bitmap"/>.
    /// </returns>
    public static Bitmap ConvertToAvaloniaBitmap(this SKBitmap bitmap)
    {
        var image = SKImage.FromPixels(bitmap.PeekPixels());
        var encodedData = image.Encode();
        var stream = encodedData.AsStream();

        return new Bitmap(stream);
    }
}