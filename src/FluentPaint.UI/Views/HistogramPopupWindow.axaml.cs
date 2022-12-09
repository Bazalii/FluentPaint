using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace FluentPaint.UI.Views;

public partial class HistogramPopupWindow : Window
{
    public HistogramPopupWindow()
    {
        InitializeComponent();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    public void AddHistogram(List<int> histogram, string histogramName)
    {
        var bar = MainPlot.Plot.AddBar(values: histogram.Select(Convert.ToDouble).ToArray(),
            positions: GenerateContinuousPositions(256));
        bar.BarWidth = 1;

        MainPlot.Plot.YAxis.Label("Y");
        MainPlot.Plot.XAxis.Label("X");
        MainPlot.Plot.SetAxisLimits(yMin: 0);

        MainPlot.Plot.Title(histogramName);

        MainPlot.Refresh();
    }

    private double[] GenerateContinuousPositions(int amount)
    {
        var positions = new double[amount];

        for (var i = 0; i < amount; i++)
        {
            positions[i] = i;
        }

        return positions;
    }
}