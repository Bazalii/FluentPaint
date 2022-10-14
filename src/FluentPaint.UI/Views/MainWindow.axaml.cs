using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FluentPaint.Core.Pnm;
using FluentPaint.UI.ViewModels;
using SkiaSharp;

namespace FluentPaint.UI.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        private string _filePath = string.Empty;
        private SKBitmap _file = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private async void OnLoadButtonClickCommand(object sender, RoutedEventArgs e)
        {
            var dialog = new LoadPopupWindow();

            _filePath = await dialog.ShowDialog<string>(this);

            try
            {
                _file = Pnm.ReadPnm(_filePath);
                
                this.FindControl<Image>("MainImage").Source = _file.ConvertToAvaloniaBitmap();
            }
            catch (Exception exception)
            {
                var window = new ExceptionWindow();

                var exceptionTextBox = window.FindControl<TextBlock>("ExceptionMessage");
                exceptionTextBox.Text = exception.Message;

                window.Show();
            }
        }

        private async void OnSaveButtonClickCommand(object sender, RoutedEventArgs e)
        {
            var dialog = new SavePopupWindow();

            _filePath = await dialog.ShowDialog<string>(this);

            try
            {
                Pnm.WritePnm(_filePath, _file);
            }
            catch (Exception exception)
            {
                var window = new ExceptionWindow();

                var exceptionTextBox = window.FindControl<TextBlock>("ExceptionMessage");
                exceptionTextBox.Text = exception.Message;

                window.Show();
            }
        }
    }
}