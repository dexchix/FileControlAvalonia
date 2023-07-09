using Avalonia;
using Avalonia.ReactiveUI;
using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using FileControlAvalonia.Services;
using FileControlAvalonia.DataBase;
using FileControlAvalonia.Core;

namespace FileControlAvalonia
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AppBootstrapper.RegisterClasses();
            Logger.InitializeLogger();
            DataBaseCreator.InitializeDataBase();
            SettingsManager.SetStartupSettings();
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}