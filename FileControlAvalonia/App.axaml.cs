using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FileControlAvalonia.ViewModels;
using FileControlAvalonia.Views;

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
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
                CurrentApplication = desktop;
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}