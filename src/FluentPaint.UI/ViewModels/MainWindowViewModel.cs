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

namespace FluentPaint.UI.ViewModels;

/// <summary>
/// Contains business logic and data for MainWindow view.
/// </summary>
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

    /// <summary>
    /// Path to the file that will be loaded in the application.
    /// </summary>
    /// <remarks>
    /// When this property is changed loads file in RGB color space.
    /// </remarks>
    public string LoadingFilePath
    {
        get => _loadingFilePath;
        set
        {
            _loadingFilePath = value;
            var file = PnmHandler.ReadPnm(value);

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

    /// <summary>
    /// Path to the file in which current picture will be saved in chosen color space.
    /// </summary>
    /// <remarks>
    /// When this property is changed also saves file in in chosen color space.
    /// </remarks>
    public string SavingFilePath
    {
        get => _savingFilePath;
        set
        {
            _savingFilePath = value;

            Enum.TryParse(SelectedSpace, out ColorSpace colorSpace);

            PnmHandler.WritePnm(_savingFilePath, colorSpace == ColorSpace.RGB ? _rgbFile : _currentColorSpaceFile);
        }
    }

    /// <summary>
    /// Represents loaded file in RGB color space.
    /// </summary>
    public SKBitmap RgbFile
    {
        get => _rgbFile;
        set => this.RaiseAndSetIfChanged(ref _rgbFile, value);
    }

    /// <summary>
    /// Items that are displayed in ColorSpaceChoosing ComboBox of MainWindow.
    /// </summary>
    public List<ComboBoxItem> Spaces { get; set; } = new();

    /// <summary>
    /// Items that are displayed in ChannelsChoosing ComboBox of MainWindow.
    /// </summary>
    public List<ComboBoxItem> Channels { get; set; } = new();

    /// <summary>
    /// Color space that is selected by the user.
    /// </summary>
    /// <remarks>
    /// When this property is changed also converts file to new color space.
    /// </remarks>
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

    /// <summary>
    /// Color channels that will be shown.
    /// </summary>
    public string SelectedChannels { get; set; } = "Channel";

    /// <summary>
    /// Filters image in current color space so that output contains only chosen channels using <see cref="SelectedSpace"/> and <see cref="SelectedChannels"/>.
    /// </summary>
    /// <returns>
    /// Bitmap that contains only chosen channels.
    /// </returns>
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

    /// <summary>
    /// Adds <see cref="ComboBoxItem"/> for each color space name in <see cref="Spaces"/> that are displayed in MainWindow.
    /// </summary>
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

    /// <summary>
    /// Adds <see cref="ComboBoxItem"/> for each channels combination in <see cref="Channels"/> that are displayed in MainWindow.
    /// </summary>
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