using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using FileControlAvalonia.Services;
using FileControlAvalonia.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;
using Avalonia.ReactiveUI;
using Splat;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Security.Policy;

namespace FileControlAvalonia.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            Deactivated += DeactivatedWindow;
            this.WhenActivated(d => d(ViewModel!.ShowDialogInfoWindow.RegisterHandler(InfoWindowShowDialog)));
            this.WhenActivated(d => d(ViewModel!.ShowDialogSettingsWindow.RegisterHandler(SettingsWindowShowDialog)));
            this.WhenActivated(d => d(ViewModel!.ShowDialogFileExplorerWindow.RegisterHandler(FileExplorerWindowShodDialog)));
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        #region ShowDialogs
        private async Task InfoWindowShowDialog(InteractionContext<InfoWindowViewModel, InfoWindowViewModel?> interaction)
        {
            var dialog = new InfoWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<InfoWindowViewModel?>(this);
            interaction.SetOutput(result);
        }

        private async Task SettingsWindowShowDialog(InteractionContext<SettingsWindowViewModel, SettingsWindowViewModel?> interaction)
        {
            var dialog = new SettingsWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<SettingsWindowViewModel?>(this);
            interaction.SetOutput(result);
        }

        private async Task FileExplorerWindowShodDialog(InteractionContext<FileExplorerWindowViewModel, FileExplorerWindowViewModel?> interaction)
        {
            var dialog = new FileExplorerWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<FileExplorerWindowViewModel?>(this);
            interaction.SetOutput(result);
        }

        #endregion

        private void DeactivatedWindow(object? sender, EventArgs e)
        {
            string activeWindow = WindowsAPI.GetActiveProcessName();
            if (activeWindow != WindowsAPI.programProcessName)
            {
                var window = App.CurrentApplication.Windows;
                for (int i = 1; i < window.Count; i++)
                {
                    window[i].Close();
                }
                window[0].Hide();
            }
        }


        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            Position = new PixelPoint(WindowAssistant.X_Coordinate, WindowAssistant.Y_Coordinate);
            CanResize = false;
        }
    }
}