using SkiaSharp;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace FluentPaint.UI;

public static class SKBitmapExtensions
{
    public static Bitmap ConvertToAvaloniaBitmap(this SKBitmap bitmap)
    {
        var image = SKImage.FromPixels(bitmap.PeekPixels());
        var encodedData = image.Encode();
        var stream = encodedData.AsStream();
        
        return new Bitmap(stream);
    }
}