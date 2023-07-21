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
        public static int CountFiles { get; set; }
        public static void CreateEtalon(FileTree fileTree)
        {
            var converter = new DataBase.DataBaseConverter();
            var etalonFilesCollection = converter.ConvertFormatFileTreeToDB(fileTree);

            var options = new SQLiteConnectionString("FileIntegrityDB.db", true, "password");
            using (var connection = new SQLiteConnection(options))
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
            var options = new SQLiteConnectionString("FileIntegrityDB.db", true, "password");

            using (var connection = new SQLiteConnection(options))
            {
                var command = new SQLiteCommand(connection)
                {
                    CommandText = "SELECT ID, ParentID, Name, Path, LastUpdate, Version, HashSum FROM FilesTable"
                };
                etalon = command.ExecuteQuery<FileDB>();
            }
            var converter = new DataBaseConverter();
            var etalonInDBContext = converter.ConvertFormatDBToFileTree(etalon);
            CountFiles = etalon.Count;

            return etalonInDBContext;
        }

    }
}
