using System;
using System.Collections.Generic;
using Avalonia.Controls;
using FluentPaint.Core.Channels;
using FluentPaint.Core.Channels.Implementations;
using FluentPaint.Core.Converters;
using FluentPaint.Core.Enums;
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
        private readonly IChannelGetter _channelGetter = new ChannelGetter();

        private readonly List<string> _colorSpaceNames = new()
            { "RGB", "HSL", "HSV", "YCbCr601", "YCbCr709", "YCoCg", "CMY" };

        private readonly List<string> _channels = new()
            { "First", "Second", "Third", "FirstAndSecond", "FirstAndThird", "SecondAndThird", "All" };

        public MainWindowViewModel()
        {
            SetColorSpaces();
            SetChannels();
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

        public List<ComboBoxItem> Spaces { get; set; } = new();

        public List<ComboBoxItem> Channels { get; set; } = new();

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

        public string SelectedChannels { get; set; } = "Channel";

        public SKBitmap GetBitmapChannels()
        {
            Enum.TryParse(SelectedSpace, out ColorSpace colorSpace);
            Enum.TryParse(SelectedChannels, out ColorChannels colorChannels);

            if (colorSpace == ColorSpace.RGB)
            {
                return _channelGetter.GetChannels(_rgbFile, colorChannels);
            }

            var convertor = _converterFactory.GetConverter(colorSpace);

            var modifiedCurrentColorSpaceBitmap = _channelGetter.GetChannels(_currentColorSpaceFile, colorChannels);

            return convertor.ToRgb(modifiedCurrentColorSpaceBitmap);
        }

        private void SetColorSpaces()
        {
            _colorSpaceNames.ForEach(name =>
            {
                var comboBoxItem = new ComboBoxItem
                {
                    Content = name
                };

                Spaces.Add(comboBoxItem);
            });
        }

        private void SetChannels()
        {
            _channels.ForEach(name =>
            {
                var comboBoxItem = new ComboBoxItem
                {
                    Content = name
                };

                Channels.Add(comboBoxItem);
            });
        }
    }
}