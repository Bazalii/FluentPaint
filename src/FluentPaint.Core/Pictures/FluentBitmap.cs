using SkiaSharp;

namespace FluentPaint.Core.Pictures;

public class FluentBitmap : SKBitmap
{
    public FluentBitmap(int width, int height) : base(width, height)
    {
    }
    
    public float Gamma { get; set; } = 1.0f;
}