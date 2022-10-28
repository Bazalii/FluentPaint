using SkiaSharp;
using FluentPaint.Core.Enums;

namespace FluentPaint.Core.Channels;

public interface IChannelGetter
{
    SKBitmap GetChannels(SKBitmap bitmap, ColorChannels channels);
}