using System;
using System.Collections.Generic;
using System.Configuration;
//using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileControlAvalonia.Core;
using Splat;
using SQLite;
using SQLitePCL;

namespace FileControlAvalonia.DataBase
{
    public static class DataBaseManager
    {
        private static string databaseFileName = "FileIntegrityDB.db";
        private static string currentDirectory = Directory.GetCurrentDirectory();
        private static string databasePath = Path.Combine(currentDirectory, databaseFileName);
        public static void InitializeDataBase()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (!File.Exists(databasePath))
                {
                    CreateDataBase();
                }
            }
            else
            {
                if (!File.Exists(databasePath))
                {
                    CreateDataBase();
                }
            }

        }
        private static void CreateDataBase()
        {
            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                string createFilesTableQuery = @"CREATE TABLE IF NOT EXISTS FileDB(
                                                 ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                 Name TEXT,
                                                 Path TEXT,
                                                 ELastUpdate TEXT,
                                                 EVersion TEXT,
                                                 EHashSum TEXT,
                                                 FLastUpdate TEXT,
                                                 FVersion TEXT,
                                                 FHashSum TEXT,
                                                 ParentPath TEXT,
                                                 Status TEXT,
                                                 IsDirectory BOOLEAN
                                             );";

                string createCheksTableQuery = @"CREATE TABLE IF NOT EXISTS CheksTable (
                                                 ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                 Creator VARCHAR(512),
                                                 Date VARCHAR(512),
                                                 DateLastCheck VARCHAR(512),
                                                 TotalFiles INTEGER,
                                                 Checked INTEGER,
                                                 PartialChecked INTEGER,
                                                 FailedChecked INTEGER,
                                                 NoAccess INTEGER,
                                                 NotFound INTEGER,
                                                 NotChecked INTEGER
                                             );";


                string insertValuesInfo = "INSERT INTO CheksTable (Creator, Date, DateLastCheck, TotalFiles, Checked, PartialChecked, FailedChecked, NoAccess, NotFound, NotChecked) " +
                               $"VALUES ('', '', '', '0', '0', '0', '0', '0', '0', '0');";


                var command = new SQLiteCommand(connection);

                command.CommandText = createFilesTableQuery;
                command.ExecuteNonQuery();
                command.CommandText = createCheksTableQuery;
                command.ExecuteNonQuery();
                command.CommandText = insertValuesInfo;
                command.ExecuteNonQuery();
            }
        }
        public static void ChangePasswordDataBase(string newPassword)
        {
            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                try
                {
                    var command = new SQLiteCommand(connection)
                    {
                        CommandText = $"PRAGMA rekey = '{newPassword}';"
                    };
                    command.ExecuteNonQuery();
                }
                catch
                {

                }
                DataBaseOptions.SetOptions();
            }
        }
    }
}
