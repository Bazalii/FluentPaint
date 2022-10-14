using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace FluentPaint.UI.Views;

public partial class LoadPopupWindow : Window
{
    public LoadPopupWindow()
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
        var userInput = this.FindControl<TextBox>("LoadFilePath").Text;

        Close(userInput);
    }
}