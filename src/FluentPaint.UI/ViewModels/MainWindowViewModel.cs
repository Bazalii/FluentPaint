using System.Collections.Generic;
using Avalonia.Controls;
using FluentPaint.Core.Pnm;
using ReactiveUI;
using SkiaSharp;

namespace FluentPaint.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _loadingFilePath = string.Empty;
        private string _savingFilePath = string.Empty;
        private SKBitmap _file = new();

        private readonly List<string> _colorSpaceNames = new()
            { "RGB", "HSL", "HSV", "YCbCr.601", "YCbCr.709", "YCoCg", "CMY" };

        public MainWindowViewModel()
        {
            SetColorSpaces();
        }

        public string LoadingFilePath
        {
            get => _loadingFilePath;
            set
            {
                _loadingFilePath = value;
                File = Pnm.ReadPnm(value);
            }
        }

        public string SavingFilePath
        {
            get => _savingFilePath;
            set
            {
                _savingFilePath = value;
                Pnm.WritePnm(_savingFilePath, _file);
            }
        }

        public SKBitmap File
        {
            get => _file;
            private set => this.RaiseAndSetIfChanged(ref _file, value);
        }

        public List<ComboBoxItem> Items { get; set; } = new();

        public string PlaceholderText { get; set; } = "Space choosing";

        private void SetColorSpaces()
        {
            _colorSpaceNames.ForEach(name =>
            {
                var comboBoxItem = new ComboBoxItem
                {
                    Content = name
                };

                Items.Add(comboBoxItem);
            });
        }
    }
}