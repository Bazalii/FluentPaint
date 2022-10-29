using SkiaSharp;

namespace FluentPaint.Core.Converters;
/// <summary>
/// Provides methods to convert picture from rgb to corresponding color space, and in the opposite direction.
/// </summary>
public interface IConverter
{
    /// <summary>
    /// Provides methods to convert picture from rgb to corresponding color space.
    /// </summary>
    /// <param name="bitmap"> bitmap containing all pixels of the picture.</param>
    /// <returns> converted bitmap</returns>
    SKBitmap FromRgb(SKBitmap bitmap);
    
    /// <summary>
    /// Provides methods to convert picture from corresponding color space to rgb.
    /// </summary>
    /// <param name="bitmap"> bitmap containing all pixels of the picture.</param>
    /// <returns> converted bitmap.</returns>
    SKBitmap ToRgb(SKBitmap bitmap);
}