using Avalonia;
using Avalonia.Controls;
using FileControlAvalonia.Helper;
using System;

namespace FileControlAvalonia.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            Position = new PixelPoint(WindowAssistant.X_Coordinate + 400, WindowAssistant.Y_Coordinate + 30);
            CanResize = false;
        }
    }
}