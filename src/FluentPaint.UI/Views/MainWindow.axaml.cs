using System;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using FluentPaint.UI.ViewModels;

namespace FluentPaint.UI.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }

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
    }
}