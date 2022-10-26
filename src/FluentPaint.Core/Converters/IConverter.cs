using SkiaSharp;

namespace FluentPaint.Core.Converters;

public interface IConverter
{
    public SKBitmap FromRgb(SKBitmap bitmap);
    public SKBitmap ToRgb(SKBitmap bitmap);
}