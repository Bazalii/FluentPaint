using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FluentPaint.UI.Models;

namespace FluentPaint.UI.Views;

public partial class FilterParametersPopupWindow : Window
{
    public FilterParametersPopupWindow()
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
        var sigma = this.FindControl<TextBox>("Sigma").Text;
        var sharpness = this.FindControl<TextBox>("Sharpness").Text;
        var radius = this.FindControl<TextBox>("Radius").Text;
        var limit = this.FindControl<TextBox>("Limit").Text;
        var threshold = this.FindControl<TextBox>("Threshold").Text;

        var filterParameters = new FilterParameters
        {
            Sigma = float.Parse(sigma, CultureInfo.InvariantCulture.NumberFormat),
            Sharpness = float.Parse(sharpness, CultureInfo.InvariantCulture.NumberFormat),
            Radius = int.Parse(radius),
            Limit = int.Parse(limit),
            Threshold = byte.Parse(threshold)
        };

        Close(filterParameters);
    }
}