using FluentPaint.Core.Converters.Implementations;
using FluentPaint.Core.Enums;

namespace FluentPaint.Core.Converters;

/// <summary>
/// Provides converters for different color spaces.
/// </summary>
public class ConverterFactory
{
    /// <summary>
    /// Provides suitable converter based on provided color space.
    /// </summary>
    /// <param name="colorSpace"> Color space of the picture.</param>
    /// <returns>
    /// Converter to handle pictures of provided color space.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when provided color space is incorrect.
    /// </exception>
    public IConverter GetConverter(ColorSpace colorSpace)
    {
        IConverter converter = colorSpace switch
        {
            ColorSpace.HSL => new HslConverter(),
            ColorSpace.HSV => new HsvConverter(),
            ColorSpace.YCbCr601 => new YCbCr601Converter(),
            ColorSpace.YCbCr709 => new YCbCr709Converter(),
            ColorSpace.YCoCg => new YCoCgConverter(),
            ColorSpace.CMY => new CmyConverter(),
            _ => throw new ArgumentException("Error: This color space is not supported")
        };

        return converter;
    }
}