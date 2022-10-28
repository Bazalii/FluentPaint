using SkiaSharp;

namespace FluentPaint.Core.Converters;

public interface IConverter
{
    SKBitmap FromRgb(SKBitmap bitmap);
    SKBitmap ToRgb(SKBitmap bitmap);
}