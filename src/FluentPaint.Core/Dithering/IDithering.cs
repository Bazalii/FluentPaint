using System.Drawing;
using FluentPaint.Core.Pictures;
using SkiaSharp;

namespace FluentPaint.Core.Dithering;

public interface IDithering
{
    FluentBitmap Dithering(FluentBitmap bitmap, int bitDepth);
}