using FluentPaint.Core.Pictures;
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
    /// <param name="bitmap"> <see cref="FluentBitmap"/> that contains all pixels of the picture. </param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    FluentBitmap FromRgb(FluentBitmap bitmap);
    
    /// <summary>
    /// Converts picture from corresponding color space to rgb.
    /// </summary>
    /// <param name="bitmap"> <see cref="FluentBitmap"/> that contains all pixels of the picture. </param>
    /// <returns>
    /// Converted bitmap.
    /// </returns>
    FluentBitmap ToRgb(FluentBitmap bitmap);
}