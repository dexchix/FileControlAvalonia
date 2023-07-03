using Avalonia;
using Avalonia.ReactiveUI;
using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace FileControlAvalonia
{
    internal class Program
    {
        private static LoggingConfiguration config = new LoggingConfiguration();
        public static Logger logger = LogManager.GetCurrentClassLogger();
        private static FileTarget fileTarget = new FileTarget("logfile")
        {
            FileName = "${basedir}/logs/${shortdate}.log",
            Layout = "${longdate} ${level} ${message} ${exception:format=ToString}",
            ArchiveAboveSize = 1073741824L,
            MaxArchiveDays = 7
        };
        private static LoggingRule rule = new LoggingRule("*", LogLevel.Debug, fileTarget);


        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            config.AddTarget(fileTarget);
            config.AddRule(rule);
            LogManager.Configuration = config;
            DataBase.DataBaseCreator.InitializeDataBase();
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}