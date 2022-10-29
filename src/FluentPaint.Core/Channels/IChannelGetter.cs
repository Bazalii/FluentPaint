using SkiaSharp;
using FluentPaint.Core.Enums;

namespace FluentPaint.Core.Channels;

/// <summary>
/// Provides method to get one or many color channels of pictures.
/// </summary>
public interface IChannelGetter
{
    /// <summary>
    /// Gets picture in provided color channels.
    /// </summary>
    /// <param name="bitmap"> <see cref="SKBitmap"/> that contains all pixels of the picture. </param>
    /// <param name="channels"> Channels in which the image will be displayed. </param>
    /// <returns>
    /// Bitmap with pixels with correct color channels according to provided channels.
    /// </returns>
    SKBitmap GetChannels(SKBitmap bitmap, ColorChannels channels);
}