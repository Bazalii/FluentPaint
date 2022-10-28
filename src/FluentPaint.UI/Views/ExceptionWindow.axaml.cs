using Avalonia;
using Avalonia.Controls;

namespace FluentPaint.UI.Views;

/// <summary>
/// View for the window that shows exceptions.
/// </summary>
public partial class ExceptionWindow : Window
{
    public ExceptionWindow()
    {
        InitializeComponent();

#if DEBUG
        this.AttachDevTools();
#endif
    }
}