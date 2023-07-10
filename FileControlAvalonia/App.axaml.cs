using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using FileControlAvalonia.Core;
using FileControlAvalonia.ViewModels;
using FileControlAvalonia.ViewModels.Interfaces;
using FileControlAvalonia.Views;
using ReactiveUI;
using Splat;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileControlAvalonia
{
    public partial class App : Application
    {
        public static IClassicDesktopStyleApplicationLifetime? CurrentApplication;
        private TrayIcon trayIcon;
        private IClassicDesktopStyleApplicationLifetime thisApplication;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            InitializeTrayIcon();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                thisApplication = desktop;

                var mainWindowViewModel = Locator.Current.GetService<MainWindowViewModel>();
                desktop.MainWindow = Locator.Current.GetService<MainWindow>();

                desktop.MainWindow!.DataContext = mainWindowViewModel;
                CurrentApplication = desktop;
            }
            base.OnFrameworkInitializationCompleted();

        }

        private void InitializeTrayIcon()
        {
            var mainWindow = Locator.Current.GetService<MainWindow>();

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var bitmap = new Bitmap(assets.Open(new Uri("avares://FileControlAvalonia/Assets/CopyModelDifferences_32x32.ico")));
            trayIcon = new TrayIcon();
            trayIcon.Icon = new WindowIcon(bitmap);
            trayIcon.IsVisible = true;
            trayIcon.ToolTipText = "ÊÎÍÒÐÎËÜ ÖÅËÎÑÒÍÎÑÒÈ ÏÎ ÂÓ";
            trayIcon.Menu = new NativeMenu()
            {
                new NativeMenuItem("Âûõîä")
                {
                   Command = ReactiveCommand.Create(() => thisApplication.Shutdown())
                }
            };
            trayIcon.Clicked += (sender, e) =>
            {
                mainWindow!.Show();
                mainWindow.WindowState = WindowState.Normal;
            };
        }
    }
}