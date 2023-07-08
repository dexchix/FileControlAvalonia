using NLog.Config;
using NLog.Targets;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia
{
    public static class Logger
    {
        private static LoggingConfiguration config = new LoggingConfiguration();
        public static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        private static FileTarget fileTarget = new FileTarget("logfile")
        {
            FileName = "${basedir}/logs/${shortdate}.log",
            Layout = "${longdate} ${level} ${message} ${exception:format=ToString}",
            ArchiveAboveSize = 1073741824L,
            MaxArchiveDays = 7
        };

        public static void InitializeLogger()
        {
            LoggingRule rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.AddTarget(fileTarget);
            config.AddRule(rule);
            LogManager.Configuration = config;
        }
    }
}
