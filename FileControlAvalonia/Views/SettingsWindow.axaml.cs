using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using FileControlAvalonia.Services;
using Splat;
using System;

namespace FileControlAvalonia.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.FindControl<TabControl>("SettingsTabControl").SelectionChanged += ResizeWindow;
        }

        private void DeactivatedWindow(object? sender, EventArgs e)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
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
            //double parentX = Locator.Current.GetService<MainWindow>().Position.X;
            //double parentY = Locator.Current.GetService<MainWindow>().Position.Y;
            //double parentWidth = Bounds.Width;
            //double parentHeight = Bounds.Height;

            //double childWidth = Bounds.Width;
            //double childHeight = Bounds.Height;

            //double childX = parentX + (parentWidth - childWidth) / 2;
            //double childY = parentY + (parentHeight - childHeight) / 2;

            //Position = new PixelPoint((int)childX, (int)childY);

            double parentX = Locator.Current.GetService<MainWindow>().Position.X;
            double parentY = Locator.Current.GetService<MainWindow>().Position.Y;
            double parentBoundsWidth = Locator.Current.GetService<MainWindow>().Bounds.Width;
            double parentBoundsHeight = Locator.Current.GetService<MainWindow>().Bounds.Height;

            double childX = parentX + (parentBoundsWidth - Bounds.Width) / 2;
            double childY = parentY + (parentBoundsHeight - Bounds.Height) / 2;

            Position = new PixelPoint((int)childX, (int)childY);   
        }
        private void ResizeWindow(object? sender, SelectionChangedEventArgs e)
        {
            var tabControl = sender as TabControl;
            int index = tabControl.SelectedIndex;
            switch (index)
            {
                case 0:
                    {
                        this.Height = 330;
                        break;
                    }
                case 1:
                    {
                        this.Height = 400;
                        break;
                    }
                case 2:
                    {
                        this.Height = 295;
                        break;
                    }
            }

        }
    }
}
