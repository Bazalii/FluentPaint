using SkiaSharp;

namespace FluentPaint.Core.Converters;
/// <summary>
/// Provides methods to convert picture from rgb to corresponding color space, and in the opposite direction.
/// </summary>
public interface IConverter
{
    /// <summary>
    /// Provides method to convert picture from rgb to corresponding color space.
    /// </summary>
    /// <param name="bitmap"> <see cref="SKBitmap"/> Bitmap containing all pixels of the picture. </param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    SKBitmap FromRgb(SKBitmap bitmap);
    
    /// <summary>
    /// Provides method to convert picture from corresponding color space to rgb.
    /// </summary>
    /// <param name="bitmap"> <see cref="SKBitmap"/> Bitmap containing all pixels of the picture. </param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    SKBitmap ToRgb(SKBitmap bitmap);
}