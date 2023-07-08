using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileControlAvalonia.ViewModels;
using Splat;

namespace FileControlAvalonia.Services
{
    public static class AppBootstrapper
    {
        public static void RegisterClasses()
        {
            Locator.CurrentMutable.Register(() => new FileExplorerWindowViewModel(), typeof(FileExplorerWindowViewModel));
            Locator.CurrentMutable.RegisterLazySingleton(() => new MainWindowViewModel(), typeof(MainWindowViewModel));
            Locator.CurrentMutable.RegisterLazySingleton(() => new InfoWindowViewModel(), typeof(InfoWindowViewModel));
            Locator.CurrentMutable.RegisterLazySingleton(() => new SettingsWindowViewModel(), typeof(SettingsWindowViewModel));
        }
    }
}
