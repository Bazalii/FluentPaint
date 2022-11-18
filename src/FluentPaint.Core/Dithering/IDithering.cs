using System.Drawing;
using SkiaSharp;

namespace FluentPaint.Core.Dithering;

public interface IDithering
{
    SKBitmap Dithering(SKBitmap bitmap, int bitDepth);
}