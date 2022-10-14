using SkiaSharp;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace FluentPaint.UI;

public static class SKBitmapExtensions
{
    public static Bitmap ConvertToAvaloniaBitmap(this SKBitmap bitmap)
    {
        return new Bitmap("");
    }
}