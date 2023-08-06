using Avalonia;
using Avalonia.ReactiveUI;
using System;
using NLog;
using FileControlAvalonia.Services;
using FileControlAvalonia.DataBase;
using FileControlAvalonia.SettingsApp;

namespace FileControlAvalonia
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AppBootstrapper.RegisterClasses();
            Logger.InitializeLogger();
            SettingsManager.SetStartupSettings();
            DataBaseOptions.SetOptions();
            DataBaseManager.InitializeDataBase();
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}