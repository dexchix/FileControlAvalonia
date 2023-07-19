using FileControlAvalonia.DataBase;
using FileControlAvalonia.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                                   $"VALUES ({file.id}, {file.idParent}, '{file.name}', '{file.path}', '{file.lastUpdate}', '{file.version}', '{file.hashSum}');"
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
            var etalon = new List<FileDB>();
            var options = new SQLiteConnectionString("FileIntegrityDB.db", true, "password");
            //using (var connection = new SQLiteConnection(options))
            //{
            //    var command = new SQLiteCommand(connection)
            //    {
            //        CommandText = "SELECT ID, ParentID, Name, Path, LastUpdate, Version, HashSum FROM FilesTable"
            //    };
            //    using var reader = command.;

            //    while (reader.Read())
            //    {
            //        int id = Convert.ToInt32(reader["ID"]);
            //        int parentId = Convert.ToInt32(reader["ParentID"]);
            //        string name = reader["Name"].ToString();
            //        string path = reader["Path"].ToString();
            //        string lastUpdate = reader["LastUpdate"].ToString();
            //        string version = reader["Version"].ToString();
            //        string hashSum = reader["HashSum"].ToString();

            //        etalon.Add(new FileDB(id, name, path, lastUpdate, version, hashSum, parentId));
            //    }
            //}

            var converter = new DataBaseConverter();
            var etalon1 = converter.ConvertFormatDBToFileTree(etalon);
            CountFiles = etalon.Count;

            return etalon1;
        }

    }
}
