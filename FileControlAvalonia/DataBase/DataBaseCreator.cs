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
        private static string linuxPathDataBase = @"../../DataBase/FileIntegrityDB.db";
        private static string windowsPathDataBase = @"..\..\DataBase\FileIntegrityDB.db";
        private static string connectionStringlinuxPathDataBase = @"Data Source=../../DataBase/FileIntegrityDB.db;Version=3;";
        private static string connectionStringWindowsPathDataBase = @"Data Source=..\..\DataBase\FileIntegrityDB.db;Version=3";
        public static void InitializeDataBase()
        {
            if(Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (!File.Exists(windowsPathDataBase))
                {
                    CreateFileDataBase(connectionStringWindowsPathDataBase, windowsPathDataBase);
                }
            }
            else
            {
                if (!File.Exists(linuxPathDataBase))
                {
                    CreateFileDataBase(connectionStringlinuxPathDataBase, linuxPathDataBase);
                }
            }
          
        }
        private static void CreateFileDataBase(string conectionString, string pathDataBase)
        {
            SQLiteConnection.CreateFile(pathDataBase);
            using(var connection = new SQLiteConnection(conectionString))
            {
                connection.Open();

                string createFilesTableQuery = @"CREATE TABLE IF NOT EXISTS Files (
                                                 ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                 Name TEXT,
                                                 Path TEXT,
                                                 DataChange TEXT,
                                                 HashSum TEXT
                                             );";

                string createCheksTableQuery = @"CREATE TABLE IF NOT EXISTS Cheks (
                                                 ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                 Date TEXT,
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
