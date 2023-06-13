using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FileControlAvalonia.Services;
using FileControlAvalonia.ViewModels;
using FileControlAvalonia.ViewModels.Interfaces;
using FileControlAvalonia.Views;
using Splat;

namespace FileControlAvalonia
{
    public partial class App : Application
    {
        public static IClassicDesktopStyleApplicationLifetime? CurrentApplication;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }


        //public override void OnFrameworkInitializationCompleted()
        //{
        //    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        //    {
        //        desktop.MainWindow = new MainWindow
        //        {
        //            DataContext = new MainWindowViewModel(),
        //        };
        //        CurrentApplication = desktop;
        //    }
        //    base.OnFrameworkInitializationCompleted();
        //}
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Locator.CurrentMutable.Register(() => new FileExplorerWindowViewModel(), typeof(FileExplorerWindowViewModel));
                Locator.CurrentMutable.Register(() => new InfoWindowViewModel(), typeof(InfoWindowViewModel));
                Locator.CurrentMutable.Register(() => new MainWindowViewModel(), typeof(MainWindowViewModel));
                Locator.CurrentMutable.Register(() => new SettingsWindowViewModel(), typeof(SettingsWindowViewModel));
                var mainWindowViewModel = Locator.Current.GetService<MainWindowViewModel>();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainWindowViewModel,
                };
                CurrentApplication = desktop;
                SettingsManager.SetStartupSettings();
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}