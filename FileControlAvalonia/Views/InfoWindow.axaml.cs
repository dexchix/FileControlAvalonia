using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FileControlAvalonia.Services;
using System;

namespace FileControlAvalonia.Views
{
    public partial class InfoWindow : Window
    {
        public InfoWindow()
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
            Position = new PixelPoint(WindowAssistant.X_Coordinate +100, WindowAssistant.Y_Coordinate + 120);
            CanResize = false;
        }
    }
}
