using FluentPaint.Core.Enums;
using SkiaSharp;

namespace FluentPaint.Core.Filters;

public interface IFilter
{
    SKBitmap Filter(ColorChannels channels, SKBitmap bitmap);
}