using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FluentPaint.UI.ViewModels;

namespace FluentPaint.UI.Views;

/// <summary>
/// View for window that contains all controls for interaction with user.
/// </summary>
/// <remarks>
/// Also opens new windows like <see cref="ExceptionWindow"/>, <see cref="LoadPopupWindow"/> and <see cref="SavePopupWindow"/>>.
/// </remarks>
public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }

    /// <summary>
    /// Opens <see cref="LoadPopupWindow"/> when LoadButton is clicked, loads file and shows it to user.
    /// </summary>
    /// <remarks>
    /// If an exception is caught, opens <see cref="ExceptionWindow"/>.
    /// </remarks>
    /// <param name="sender"> Object that raises the event. </param>
    /// <param name="e"> Arguments that are associated with event. </param>
    private async void OnLoadButtonClickCommand(object sender, RoutedEventArgs e)
    {
        var dialog = new LoadPopupWindow
        {
            Width = 700,
            Height = 300
        };

        try
        {
            ViewModel.LoadingFilePath = await dialog.ShowDialog<string>(this);
            MainImage.Source = ViewModel.RgbFile.ConvertToAvaloniaBitmap();
        }
        catch (Exception exception)
        {
            ShowException(exception.Message);
        }
    }

    /// <summary>
    /// Opens <see cref="LoadPopupWindow"/> when SaveButton is clicked, saves picture using <see cref="FluentPaint.UI.ViewModels.MainWindowViewModel.SavingFilePath"/>.
    /// </summary>
    /// <remarks>
    /// If an exception is caught, calls <see cref="FluentPaint.UI.Views.MainWindow.ShowException"/>.
    /// </remarks>
    /// <param name="sender"> Object that raises the event. </param>
    /// <param name="e"> Arguments that are associated with event. </param>
    private async void OnSaveButtonClickCommand(object sender, RoutedEventArgs e)
    {
        var dialog = new SavePopupWindow
        {
            Width = 700,
            Height = 300
        };

        try
        {
            ViewModel.SavingFilePath = await dialog.ShowDialog<string>(this);
        }
        catch (Exception exception)
        {
            ShowException(exception.Message);
        }
    }

    private async void OnSetGammaButtonClickCommand(object sender, RoutedEventArgs e)
    {
        var dialog = new GammaInputPopupWindow
        {
            Width = 700,
            Height = 300
        };

        try
        {
            ViewModel.CurrentGamma = await dialog.ShowDialog<float>(this);
        }
        catch (Exception exception)
        {
            ShowException(exception.Message);
        }
    }

    private async void OnConvertButtonClickCommand(object sender, RoutedEventArgs e)
    {
        var dialog = new GammaFileSavePopupWindow
        {
            Width = 700,
            Height = 300
        };

        try
        {
            ViewModel.SavingGammaFilePath = await dialog.ShowDialog<string>(this);
        }
        catch (Exception exception)
        {
            ShowException(exception.Message);
        }
    }

    private void OnAssignButtonClickCommand(object? sender, RoutedEventArgs e)
    {
        MainImage.Source = ViewModel.CurrentGammaFile.ConvertToAvaloniaBitmap();
    }

    /// <summary>
    /// Opens <see cref="ExceptionWindow"/> that contains information about caught exception.
    /// </summary>
    /// <param name="message"> Exception message. </param>
    private void ShowException(string message)
    {
        var window = new ExceptionWindow
        {
            Width = 700,
            Height = 300,
            ExceptionMessage =
            {
                Text = message
            }
        };

        window.Show();
    }

    /// <summary>
    /// Gets current channels when user selects new item using ChannelsChoosing <see cref="ComboBox"/>.
    /// </summary>
    /// <param name="sender"> Object that raises the event. </param>
    /// <param name="selectionChangedEventArgs"> Arguments that are associated with event. </param>
    private void OnChannelsChange(object? sender, SelectionChangedEventArgs selectionChangedEventArgs)
    {
        ViewModel.SelectedChannels = ((ComboBoxItem) ((ComboBox) sender).SelectedItem).Content as string;
        MainImage.Source = ViewModel.GetBitmapChannels().ConvertToAvaloniaBitmap();
    }
}