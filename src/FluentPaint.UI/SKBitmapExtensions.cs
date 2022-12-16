using FluentPaint.Core.Pictures;
using SkiaSharp;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace FluentPaint.UI;

/// <summary>
/// Contains extension methods for <see cref="FluentBitmap"/>.
/// </summary>
public static class FluentBitmapExtensions
{
    /// <summary>
    /// Converts <see cref="FluentBitmap"/> to <see cref="Bitmap"/>.
    /// </summary>
    /// <param name="bitmap"> <see cref="FluentBitmap"/> to convert. </param>
    /// <returns>
    /// Converted <see cref="Bitmap"/>.
    /// </returns>
    public static Bitmap ConvertToAvaloniaBitmap(this FluentBitmap bitmap)
    {
        var image = SKImage.FromPixels(bitmap.PeekPixels());
        var encodedData = image.Encode();
        var stream = encodedData.AsStream();

        return new Bitmap(stream);
    }
}