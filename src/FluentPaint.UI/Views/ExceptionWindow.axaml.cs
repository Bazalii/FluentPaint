using Avalonia;
using Avalonia.Controls;

namespace FluentPaint.UI.Views;

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