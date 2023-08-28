using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FileControlAvalonia.Services;
using Splat;
using System;

namespace FileControlAvalonia.Views
{
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
            //Deactivated += DeactivatedWindow;
        }

        private void DeactivatedWindow(object? sender, EventArgs e)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
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
            else
            {
                var window = App.CurrentApplication!.Windows;
                for (int i = 1; i < window.Count; i++)
                {
                    window[i].Close();
                }
                window[0].WindowState = WindowState.Minimized;
                MainWindow.IsChildWindowOpen = false;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            double parentX = Locator.Current.GetService<MainWindow>().Position.X;
            double parentY = Locator.Current.GetService<MainWindow>().Position.Y;
            double parentBoundsWidth = Locator.Current.GetService<MainWindow>().Bounds.Width;
            double parentBoundsHeight = Locator.Current.GetService<MainWindow>().Bounds.Height;

            double childX = parentX + (parentBoundsWidth - Bounds.Width) / 2;
            double childY = parentY + (parentBoundsHeight - Bounds.Height) / 2;

            Position = new PixelPoint((int)childX, (int)childY);
        }
    }
}
