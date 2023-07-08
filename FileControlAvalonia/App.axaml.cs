using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FileControlAvalonia.Core;
using FileControlAvalonia.ViewModels;
using FileControlAvalonia.ViewModels.Interfaces;
using FileControlAvalonia.Views;
using Splat;
using System.Threading.Tasks;

namespace FileControlAvalonia
{
    public partial class App : Application
    {
        public static IClassicDesktopStyleApplicationLifetime? CurrentApplication;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
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