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
using FileControlAvalonia.SettingsApp;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FileControlAvalonia.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public static bool IsChildWindowOpen { get; set; } = false;
        public Grid title;
        public TreeDataGrid treeDataGrid;
        public static double TreeDataGridWidth;

        public MainWindow()
        {
            InitializeComponent();

            title = this.FindControl<Grid>("Title");
            if (SettingsManager.AppSettings.DragAndDropWindow)
                title.PointerPressed += DragMoveWindow;
            treeDataGrid = this.FindControl<TreeDataGrid>("fileViewer");

            //TreeDataGridWidth = this.Width - 10;
            TreeDataGridWidth = SettingsManager.AppSettings.WindowWidth - 10;
            this.Opened += SetSizeWindow;

            this.WhenActivated(d => d(ViewModel!.ShowDialogInfoWindow.RegisterHandler(InfoWindowShowDialog)));
            this.WhenActivated(d => d(ViewModel!.ShowDialogSettingsWindow.RegisterHandler(SettingsWindowShowDialog)));
            this.WhenActivated(d => d(ViewModel!.ShowDialogFileExplorerWindow.RegisterHandler(FileExplorerWindowShodDialog)));


#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void SetSizeWindow(object? sender, EventArgs e)
        {
            this.Width = (double)SettingsManager.AppSettings.WindowWidth;
            this.Height = (double)SettingsManager.AppSettings.WindowHeight;

            TreeDataGridWidth = this.Width - 10;
        }
        public void ResizeWindow(double width, double height)
        {
            Width = width;
            Height = height;
            TreeDataGridWidth = Width - 10;
        }
        public void DragMoveWindow(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            this.BeginMoveDrag(e);
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
            if(Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                string activeWindow = WindowsAPI.GetActiveProcessName();
                if (activeWindow != WindowsAPI.programProcessName)
                {
                    var window = App.CurrentApplication!.Windows;
                    for (int i = 1; i < window.Count; i++)
                    {
                        window[i].Close();
                    }
                    window[0].Hide();
                }
            }
            else
            {
                if (MainWindow.IsChildWindowOpen == false)
                {
                    var window = App.CurrentApplication!.Windows;
                    for (int i = 1; i < window.Count; i++)
                    {
                        window[i].Close();
                    }
                    window[0].WindowState = WindowState.Minimized;
                }
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