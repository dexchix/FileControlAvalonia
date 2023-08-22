using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileControlAvalonia.DataBase;
using FileControlAvalonia.ViewModels;
using FileControlAvalonia.Views;
using Splat;

namespace FileControlAvalonia.Services
{
    public static class AppBootstrapper
    {
        public static void RegisterClasses()
        {
            Locator.CurrentMutable.Register(() => new FileExplorerWindowViewModel(), typeof(FileExplorerWindowViewModel));
            Locator.CurrentMutable.Register(() => new SettingsWindowViewModel(), typeof(SettingsWindowViewModel));
            Locator.CurrentMutable.RegisterLazySingleton(() => new MainWindowViewModel(), typeof(MainWindowViewModel));
            Locator.CurrentMutable.RegisterLazySingleton(() => new InfoWindowViewModel(), typeof(InfoWindowViewModel));
            Locator.CurrentMutable.RegisterLazySingleton(() => new MainWindow(), typeof(MainWindow));
        }
    }
}
