using ReactiveUI;
using SkiaSharp;

namespace FluentPaint.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private SKBitmap _file = new();

        public string FilePath { get; set; } = string.Empty;

        public SKBitmap File
        {
            get => _file;
            private set => this.RaiseAndSetIfChanged(ref _file, value);
        }
    }
}