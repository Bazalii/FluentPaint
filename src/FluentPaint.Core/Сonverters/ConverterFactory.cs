﻿using FluentPaint.Core.Pnm;
using FluentPaint.Core.Сonverters.Implementations;

namespace FluentPaint.Core.Сonverters;

public class ConverterFactory
{
    public IConverter GetConverter(ColorSpace colorSpace)
    {
        IConverter converter = colorSpace switch
        {
            ColorSpace.HSL => new HslConverter(),
            ColorSpace.HSV => new HsvConverter(),
            ColorSpace.YCbCr601 => new YCbCr601Converter(),
            ColorSpace.YCbCr709 => new YCbCr709Converter(),
            ColorSpace.YCoCg => new YcocgConver(),
            ColorSpace.CMY => new CmyConverter(),
            _ => throw new ArgumentException("Error: This color space is not supported")
        };

        return converter;
    }
}