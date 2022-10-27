using System;
using System.Collections.Generic;
using Avalonia.Controls;
using FluentPaint.Core.Converters;
using FluentPaint.Core.Pnm;
using ReactiveUI;
using SkiaSharp;

namespace FluentPaint.UI.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string _loadingFilePath = string.Empty;
        private string _savingFilePath = string.Empty;
        private string _selectedSpace = "Space choosing";

        private SKBitmap? _rgbFile;
        private SKBitmap _currentColorSpaceFile = new();

        private readonly ConverterFactory _converterFactory = new();

        private readonly List<string> _colorSpaceNames = new()
            { "RGB", "HSL", "HSV", "YCbCr601", "YCbCr709", "YCoCg", "CMY" };

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
                var file = Pnm.ReadPnm(value);

                Enum.TryParse(SelectedSpace, out ColorSpace colorSpace);

                if (colorSpace == ColorSpace.RGB)
                {
                    RgbFile = file;
                }
                else
                {
                    var convertor = _converterFactory.GetConverter(colorSpace);

                    RgbFile = convertor.ToRgb(file);
                }
            }
        }

        public string SavingFilePath
        {
            get => _savingFilePath;
            set
            {
                _savingFilePath = value;
                
                Enum.TryParse(SelectedSpace, out ColorSpace colorSpace);

                Pnm.WritePnm(_savingFilePath, colorSpace == ColorSpace.RGB ? _rgbFile : _currentColorSpaceFile);
            }
        }

        public SKBitmap RgbFile
        {
            get => _rgbFile;
            set => this.RaiseAndSetIfChanged(ref _rgbFile, value);
    }

        public List<ComboBoxItem> Items { get; set; } = new();

        public string SelectedSpace
        {
            get => _selectedSpace;
            set
            {
                _selectedSpace = value;

                if (_rgbFile is null) return;

                Enum.TryParse(SelectedSpace, out ColorSpace colorSpace);

                if (colorSpace == ColorSpace.RGB) return;
                
                var convertor = _converterFactory.GetConverter(colorSpace);
                _currentColorSpaceFile = convertor.FromRgb(_rgbFile);
            }
        }

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