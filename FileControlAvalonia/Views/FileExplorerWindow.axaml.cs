using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FileControlAvalonia.ViewModels;
using System;
using System.Timers;
using FileControlAvalonia.Services;

namespace FileControlAvalonia.Views
{
    public partial class FileExplorerWindow : Window
    {
        private Timer timer;
        public FileExplorerWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            Position = new PixelPoint(WindowAssistant.X_Coordinate + 400, WindowAssistant.Y_Coordinate + 70);
            CanResize = false;
        }
    }
}
