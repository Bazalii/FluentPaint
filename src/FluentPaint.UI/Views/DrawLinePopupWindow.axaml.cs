using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentPaint.UI.Models;
using SkiaSharp;

namespace FluentPaint.UI.Views;

public partial class DrawLinePopupWindow : Window
{
    public DrawLinePopupWindow()
    {
        InitializeComponent();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void OnOkButtonClickCommand(object sender, RoutedEventArgs e)
    {
        var startPointInput = this.FindControl<TextBox>("StartPoint").Text;
        var endPointInput = this.FindControl<TextBox>("EndPoint").Text;
        var colorInput = this.FindControl<TextBox>("Color").Text;

        var startCoordinates = startPointInput.Split(' ').Select(element => Convert.ToInt32(element)).ToArray();
        var endCoordinates = endPointInput.Split(' ').Select(element => Convert.ToInt32(element)).ToArray();
        var colorElements = colorInput.Split(' ').Select(element => Convert.ToByte(element)).ToArray();

        var color = new SKColor(colorElements[0], colorElements[0], colorElements[0]);

        Close(new LineDrawer(startCoordinates[0], startCoordinates[1], endCoordinates[0], endCoordinates[1], color));
    }
}