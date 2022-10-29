using SkiaSharp;

namespace FluentPaint.Core.Converters;
/// <summary>
/// Provides methods to convert picture from rgb to corresponding color space and in the opposite direction.
/// </summary>
public interface IConverter
{
    /// <summary>
    /// Converts picture from rgb to corresponding color space.
    /// </summary>
    /// <param name="bitmap"> <see cref="SKBitmap"/> that contains all pixels of the picture. </param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    SKBitmap FromRgb(SKBitmap bitmap);
    
    /// <summary>
    /// Converts picture from corresponding color space to rgb.
    /// </summary>
    /// <param name="bitmap"> <see cref="SKBitmap"/> that contains all pixels of the picture. </param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    SKBitmap ToRgb(SKBitmap bitmap);
}