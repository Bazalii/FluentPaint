using FluentPaint.Core.Enums;
using FluentPaint.Core.Pictures;
using SkiaSharp;

namespace FluentPaint.Core.Filters;

public interface IFilter
{
    FluentBitmap Filter(ColorChannels channels, FluentBitmap bitmap);
}