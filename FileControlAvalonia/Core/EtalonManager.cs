using FileControlAvalonia.DataBase;
using FileControlAvalonia.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus;

namespace FileControlAvalonia.Core
{
    public class EtalonManager
    {
        public static FileTree CurentEtalon = GetEtalon();
        public static int CountFilesEtalon { get; set; }
        public static void CreateEtalon(FileTree fileTree)
        {
            var converter = new DataBase.DataBaseConverter();
            var etalonFilesCollection = converter.ConvertFormatFileTreeToDB(fileTree);

            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                var commandClearTableFiles = new SQLiteCommand(connection)
                {
                    CommandText = "DELETE FROM FilesTable"
                };
                commandClearTableFiles.ExecuteNonQuery();

                foreach (var file in etalonFilesCollection)
                {
                    var insertCommandFilesTable = new SQLiteCommand(connection)
                    {
                        CommandText = "INSERT INTO FilesTable (ID, ParentID, Name, Path, LastUpdate, Version, HashSum) " +
                                   $"VALUES ({file.ID}, {file.ParentID}, '{file.Name}', '{file.Path}', '{file.LastUpdate}', '{file.Version}', '{file.HashSum}');"
                    };

                    insertCommandFilesTable.ExecuteNonQuery();
                }

                var commandClearTableCheks = new SQLiteCommand(connection)
                {
                    CommandText = "DELETE FROM CheksTable"
                };
                commandClearTableCheks.ExecuteNonQuery();

                var insertCommandChecksTable = new SQLiteCommand(connection)
                {
                    CommandText = "INSERT INTO CheksTable (ID, Creator, Date) " +
                                   $"VALUES ({1}, 'Admin' , '{DateTime.Now.ToString()}');"
                };
                insertCommandChecksTable.ExecuteNonQuery();
            }
        }

        public static FileTree GetEtalon()
        {
            List<FileDB> etalon;

            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                var command = new SQLiteCommand(connection)
                {
                    CommandText = "SELECT ID, ParentID, Name, Path, LastUpdate, Version, HashSum FROM FilesTable"
                };
                etalon = command.ExecuteQuery<FileDB>();
            }
            var converter = new DataBaseConverter();
            var etalonInDBContext = converter.ConvertFormatDBToFileTree(etalon);
            CountFilesEtalon = etalon.Count;
            FileTree._counter = -1;
            return etalonInDBContext;
        }
        public static void AddFileInDB(FileTree file)
        {
            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                var insertCommandFilesTable = new SQLiteCommand(connection)
                {
                    CommandText = "INSERT INTO FilesTable (ID, ParentID, Name, Path, LastUpdate, Version, HashSum) " +
                           $"VALUES ({file.ID}, {file.ParentID}, '{file.Name}', '{file.Path}', '{file.FLastUpdate}', '{file.FVersion}', '{file.FHash}');"
                };
                insertCommandFilesTable.ExecuteNonQuery();
                if (file.Children != null)
                    AddChildrenInDB(file);
            }
        }
        private static void AddChildrenInDB(FileTree file)
        {
            foreach (var child in file.Children)
            {
                AddFileInDB(child);
            }
        }
        public static void DeliteFileInDB(FileTree file)
        {
            if (file.Children != null)
            {
                var listDelitedFiles = new DataBaseConverter().ConvertFormatFileTreeToDB(file);
                using (var connection = new SQLiteConnection(DataBaseOptions.Options))
                {
                    foreach (var diletedFile in listDelitedFiles)
                    {
                        var insertCommandFilesTable = new SQLiteCommand(connection)
                        {
                            CommandText = $"DELETE FROM FilesTable WHERE Path = '{diletedFile.Path}';"
                        };
                        insertCommandFilesTable.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                using (var connection = new SQLiteConnection(DataBaseOptions.Options))
                {
                    var insertCommandFilesTable = new SQLiteCommand(connection)
                    {
                        CommandText = $"DELETE FROM FilesTable WHERE Path = '{file.Path}';"
                    };
                    insertCommandFilesTable.ExecuteNonQuery();
                }
            }
        }
    }
}
