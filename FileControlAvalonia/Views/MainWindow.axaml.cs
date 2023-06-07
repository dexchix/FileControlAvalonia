using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using FileControlAvalonia.Services;

namespace FileControlAvalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Deactivated += DeactivatedWindow;
        }

        private void DeactivatedWindow(object? sender, EventArgs e)
        {
            string activeWindow = WindowsAPI.GetActiveProcessName();
            if (activeWindow != WindowsAPI.programProcessName) App.CurrentApplication!.Shutdown();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            Position = new PixelPoint(WindowAssistant.X_Coordinate, WindowAssistant.Y_Coordinate);
            CanResize = false;
        }
    }
}