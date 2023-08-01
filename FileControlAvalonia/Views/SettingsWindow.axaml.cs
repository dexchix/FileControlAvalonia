using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using FileControlAvalonia.Services;
using System;

namespace FileControlAvalonia.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            //Deactivated += DeactivatedWindow;

            //var s1 = new Style(x => x.OfType<TextBox>())
            //{
            //    Setters = {
            //    new Setter(TextBox.BorderBrushProperty, new SolidColorBrush(Colors.Red)),
            //        new Setter(TextBox.BorderThicknessProperty, new Thickness(1)),
            //        new Setter(TextBox.BackgroundProperty, new SolidColorBrush(Colors.Gray))
            //    }
            //};


            //var s2 = new Style(x => (x.OfType<TextBox>().Class(":pointerover")).Template().OfType<Border>())
            //{
            //    Setters =
            //    {
            //         new Setter(TextBox.BorderBrushProperty, new SolidColorBrush(Colors.Red)),
            //         new Setter(TextBox.BorderThicknessProperty, new Thickness(1)),
            //         new Setter(TextBox.BackgroundProperty, new SolidColorBrush(Colors.Gray))
            //    }
            //};


            //var s3 = new Style(x => (x.OfType<TextBox>().Class(":focus")).Template().OfType<Border>())
            //{
            //    Setters =
            //    {
            //         new Setter(TextBox.BorderBrushProperty, new SolidColorBrush(Colors.Red)),
            //         new Setter(TextBox.BorderThicknessProperty, new Thickness(1)),
            //         new Setter(TextBox.BackgroundProperty, new SolidColorBrush(Colors.Gray))
            //    }
            //};


            //var textBox = this.FindControl<TextBox>("PasswordBox");
            //textBox.Styles.Add(s1);
            //textBox.Styles.Add(s2);
            //textBox.Styles.Add(s3);

            
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
            Position = new PixelPoint(WindowAssistant.X_Coordinate + 350, WindowAssistant.Y_Coordinate + 10);
            CanResize = false;
        }
    }
}
