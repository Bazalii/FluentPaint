using SkiaSharp;

namespace FluentPaint.Core.Сonverters;

public interface IConverter
{
    public SKBitmap FromRgb(SKBitmap bitmap);
    public SKBitmap ToRgb(SKBitmap bitmap);
}