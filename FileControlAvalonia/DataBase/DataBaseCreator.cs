using System;
using System.Collections.Generic;
using System.Configuration;
//using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLitePCL;

namespace FileControlAvalonia.DataBase
{
    public static class DataBaseCreator
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
                    ConnectionDataBase();
                }
            }
            else
            {
                if (!File.Exists(databasePath))
                {
                    ConnectionDataBase();
                }
            }

        }
        private static void ConnectionDataBase()
        {
            var options = new SQLiteConnectionString("FileIntegrityDB.db", true, "password");
            using (var connection = new SQLiteConnection(options))
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
                                                 DateLastCheck VARCHAR(512)
                                             );";

                var command = new SQLiteCommand(connection);

                command.CommandText = createFilesTableQuery;
                command.ExecuteNonQuery();
                command.CommandText = createCheksTableQuery;
                command.ExecuteNonQuery();
            }
        }
    }
}
