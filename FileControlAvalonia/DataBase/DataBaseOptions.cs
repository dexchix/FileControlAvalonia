using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileControlAvalonia.Core;
using SQLite;
using SQLitePCL;

namespace FileControlAvalonia.DataBase
{
    public static class DataBaseOptions
    {
        public static string NameDB { get; private set; } = "FileIntegrityDB.db";
        public static string Password { get; private set; } = null;

        public static SQLiteConnectionString Options { get; set; }

        public static void InitializeDataBaseSettings()
        {
            //if (SettingsManager.AppSettings.NameTable != null || SettingsManager.AppSettings.NameTable != "")
            //    NameDB = SettingsManager.AppSettings.NameTable!;
            Password = SettingsManager.AppSettings.Password;

            Options = new SQLiteConnectionString(NameDB, true, Password);
        }
        public static void ChangeDataBaseOptions()
        {
            Password = SettingsManager.AppSettings.Password;
            Options = new SQLiteConnectionString(NameDB, false, Password);
        }
    }
}
