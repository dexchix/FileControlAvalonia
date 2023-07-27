using System;
using System.Collections.Generic;
using System.Configuration;
//using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                string createFilesTableQuery = @"CREATE TABLE IF NOT EXISTS FilesTable (
                                                 ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                 ParentID INT,
                                                 Name VARCHAR(512),
                                                 Path VARCHAR(512),
                                                 LastUpdate VARCHAR(512),
                                                 Version VARCHAR(512),
                                                 HashSum VARCHAR(512)
                                             );";

                string createCheksTableQuery = @"CREATE TABLE IF NOT EXISTS CheksTable (
                                                 ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                 Creator VARCHAR(512),
                                                 Date VARCHAR(512),
                                                 DateLastCheck VARCHAR(512),
                                                 TotalFiles VARCHAR(512),
                                                 Checked VARCHAR(512),
                                                 PartialChecked VARCHAR(512),
                                                 UnChecked VARCHAR(512),
                                                 NoAccess VARCHAR(512),
                                                 NotFound VARCHAR(512),
                                                 NotChecked VARCHAR(512)
                                                 
                                             );";

                var command = new SQLiteCommand(connection);

                command.CommandText = createFilesTableQuery;
                command.ExecuteNonQuery();
                command.CommandText = createCheksTableQuery;
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
                DataBaseOptions.ChangeDataBaseOptions();
            }
        }
    }
}
