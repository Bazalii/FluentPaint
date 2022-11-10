using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace FluentPaint.UI.Views;

public partial class GammaInputPopupWindow : Window
{
    public GammaInputPopupWindow()
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
        var userInput = this.FindControl<TextBox>("GammaValue").Text;

        Close(float.Parse(userInput, CultureInfo.InvariantCulture.NumberFormat));
    }
}