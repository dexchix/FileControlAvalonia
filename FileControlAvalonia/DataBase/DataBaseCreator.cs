using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.DataBase
{
    public static class DataBaseCreator
    {
        static string databaseFileName = "FileIntegrityDB.db";
        static string currentDirectory = Directory.GetCurrentDirectory();
        static string databasePath = Path.Combine(currentDirectory, databaseFileName);
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
            using (var connection = new SQLiteConnection("Data Source=FileIntegrityDB.db"))
            {
                connection.Open();

                string createFilesTableQuery = @"CREATE TABLE IF NOT EXISTS Files (
                                                 //ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                 //ParentID INT,
                                                 //Name VARCHAR(512),
                                                 //Path VARCHAR(512),
                                                 //LustUpdate VARCHAR(512),
                                                 //HashSum VARCHAR(512)
                                             );";

                string createCheksTableQuery = @"CREATE TABLE IF NOT EXISTS Cheks (
                                                 ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                 Creator VARCHAR(512),
                                                 Date VARCHAR(512)
                                             );";

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = createFilesTableQuery;
                    command.ExecuteNonQuery();
                    command.CommandText = createCheksTableQuery;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
