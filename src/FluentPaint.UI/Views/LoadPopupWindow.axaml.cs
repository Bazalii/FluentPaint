using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace FluentPaint.UI.Views;

/// <summary>
/// View for the window that gets path to file that will be loaded.
/// </summary>
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

    /// <summary>
    /// Closes this LoadPopupWindow when OkButton is clicked and returns user input to MainWindow.
    /// </summary>
    /// <param name="sender"> Object that raises the event. </param>
    /// <param name="e"> Arguments that are associated with event. </param>
    private void OnOkButtonClickCommand(object sender, RoutedEventArgs e)
    {
        var userInput = this.FindControl<TextBox>("LoadFilePath").Text;

        Close(userInput);
    }
}