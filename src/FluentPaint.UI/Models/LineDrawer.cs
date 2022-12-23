namespace FluentPaint.UI.Models;

using System;
using SkiaSharp;

public class LineDrawer
{
    private int _startPointX;
    private int _startPointY;
    private int _endPointX;
    private int _endPointY;

    private readonly SKColor _lineColor;

    public LineDrawer(int startPointX, int startPointY, int endPointX, int endPointY, SKColor color)
    {
        _startPointX = startPointX;
        _startPointY = startPointY;
        _endPointY = endPointY;
        _endPointX = endPointX;
        _lineColor = color;
    }

    public void Draw(SKBitmap bitmap)
    {
        var steep = Math.Abs(_endPointY - _startPointY) > Math.Abs(_endPointX - _startPointX);

        int intermediateValue;

        if (steep)
        {
            intermediateValue = _startPointX;
            _startPointX = _startPointY;
            _startPointY = intermediateValue;

            intermediateValue = _endPointX;
            _endPointX = _endPointY;
            _endPointY = intermediateValue;
        }

        if (_startPointX > _endPointX)
        {
            intermediateValue = _startPointX;
            _startPointX = _endPointX;
            _endPointX = intermediateValue;
            intermediateValue = _startPointY;
            _startPointY = _endPointY;
            _endPointY = intermediateValue;
        }

        var dx = _endPointX - _startPointX;
        var dy = _endPointY - _startPointY;
        var gradient = (double) dy / dx;

        var endPointX = _startPointX;
        var endPointY = (int) (_startPointY + gradient * (endPointX - _startPointX));
        var xGap = (int) GetRfPart(_startPointX + 0.5);

        var firstPixelX = endPointX;
        var firstPixelY = endPointY;

        if (steep)
        {
            Plot(bitmap, firstPixelY, firstPixelX, GetRfPart(endPointY) * xGap);
            Plot(bitmap, firstPixelY + 1, firstPixelX, GetFractionalPart(endPointY) * xGap);
        }
        else
        {
            Plot(bitmap, firstPixelX, firstPixelY, GetRfPart(endPointY) * xGap);
            Plot(bitmap, firstPixelX, firstPixelY + 1, GetFractionalPart(endPointY) * xGap);
        }

        var bias = endPointY + gradient;

        endPointX = _endPointX;
        endPointY = (int) (_endPointY + gradient * (endPointX - _endPointX));
        xGap = (int) GetFractionalPart(_endPointX + 0.5);

        var secondPointX = endPointX;
        var secondPointY = endPointY;

        if (steep)
        {
            Plot(bitmap, secondPointY, secondPointX, GetRfPart(endPointY) * xGap);
            Plot(bitmap, secondPointY + 1, secondPointX, GetFractionalPart(endPointY) * xGap);
        }
        else
        {
            Plot(bitmap, secondPointX, secondPointY, GetRfPart(endPointY) * xGap);
            Plot(bitmap, secondPointX, secondPointY + 1, GetFractionalPart(endPointY) * xGap);
        }

        if (steep)
        {
            for (var x = firstPixelX + 1; x <= secondPointX - 1; x++)
            {
                Plot(bitmap, bias, x, GetRfPart(bias));
                Plot(bitmap, bias + 1, x, GetFractionalPart(bias));
                bias += gradient;
            }
        }
        else
        {
            for (var x = firstPixelX + 1; x <= secondPointX - 1; x++)
            {
                Plot(bitmap, x, bias, GetRfPart(bias));
                Plot(bitmap, x, bias + 1, GetFractionalPart(bias));
                bias += gradient;
            }
        }
    }

    private void Plot(SKBitmap bitmap, double x, double y, double c)
    {
        bitmap.SetPixel((int) x, (int) y, _lineColor);
    }

    private double GetFractionalPart(double x)
    {
        if (x < 0)
        {
            return 1 - (x - Math.Floor(x));
        }

        return x - Math.Floor(x);
    }

    private double GetRfPart(double x)
    {
        return 1 - GetFractionalPart(x);
    }
}