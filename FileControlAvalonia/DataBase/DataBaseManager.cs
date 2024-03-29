﻿using System;
using System.Collections.Generic;
using System.Configuration;
//using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileControlAvalonia.Core;
using FileControlAvalonia.SettingsApp;
using ServiceLib;
using Splat;
using SQLite;
using SQLitePCL;

namespace FileControlAvalonia.DataBase
{
    public static class DataBaseManager
    {
        private static string databaseFileName = "FileIntegrityDB";
        private static string currentDirectory = Directory.GetCurrentDirectory();
        private static string databasePath = Path.Combine(currentDirectory, databaseFileName + ".db");

        public static string NameDB { get; private set; }
        public static string Password { get; private set; } = "Bzpa/123456789";
        public static SQLiteConnectionString Options { get; set; }

        public static void InitializeDataBase()
        {
            if (!File.Exists(databasePath))
                CreateDataBase();
        }

        private static void CreateDataBase()
        {
            using (var connection = new SQLiteConnection(Options))
            {
                string createFilesTableQuery = @"CREATE TABLE IF NOT EXISTS FileTree(
                                                 ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                 Name TEXT,
                                                 Path TEXT,
                                                 ELastUpdate TEXT,
                                                 EVersion TEXT,
                                                 EHash TEXT,
                                                 FLastUpdate TEXT,
                                                 FVersion TEXT,
                                                 FHash TEXT,
                                                 ParentPath TEXT,
                                                 Status INTEGER,
                                                 IsDirectory BOOLEAN,
                                                 HasChildren BOOLEAN
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
            using (var connection = new SQLiteConnection(Options))
            {
                try
                {
                    var command = new SQLiteCommand(connection)
                    {
                        CommandText = $"PRAGMA rekey = '{newPassword}';"
                    };
                    command.ExecuteNonQuery();
                }
                catch(Exception ex) 
                {
                    Logger.logger.Error($"Не удалось сменить пароль БД - {ex.Message}");
                }
                SetOptions();
            }
        }

        public static void ChangeDataBaseName(string newName)
        {
            File.Move(Path.Combine(Directory.GetCurrentDirectory(), NameDB + ".db"), Path.Combine(Directory.GetCurrentDirectory(), newName + ".db"));
            Options = new SQLiteConnectionString(newName + ".db", true, Password);
            NameDB = newName;
        }


        public static void SetOptions()
        {
            Password = SettingsManager.AppSettings.Password;
            Options = new SQLiteConnectionString(NameDB + ".db", true, Password);
        }

        public static void SetStartOptions()
        {
            NameDB = SettingsManager.AppSettings.NameDataBase;
            Password = SettingsManager.AppSettings.Password;
            Options = new SQLiteConnectionString(SettingsManager.AppSettings.NameDataBase + ".db", true, Password);
        }

        #region CRYPT_Password

        public static string CryptPassword(this string text) => EncryptingFunctions.EncryptPasswordReg(text);


        public static string DecryptPassword(this string text) => EncryptingFunctions.DecryptPasswordReg(text);
        #endregion
    }
}
