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
        public FileExplorerWindow()
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
            CanResize = false;
        }
    }
}
