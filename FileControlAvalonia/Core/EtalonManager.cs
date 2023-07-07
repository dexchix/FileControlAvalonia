using FileControlAvalonia.DataBase;
using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public class EtalonManager
    {
        public static void CreateEtalon(List<FileDB> etalonFiles)
        {
            using (var connection = new SQLiteConnection("Data Source=FileIntegrityDB.db"))
            {
                connection.Open();
                
                using var commandClearTableFiles = new SQLiteCommand("DELETE FROM FilesTable", connection);
                commandClearTableFiles.ExecuteNonQuery();

                foreach (var file in etalonFiles)
                {
                    string query = "INSERT INTO FilesTable (ID, ParentID, Name, Path, LastUpdate, Version, HashSum) " +
                                   "VALUES (@Id, @ParentId, @Name, @Path, @LastUpdate, @Version, @HashSum);";

                    using var insertCommandFilesTable = new SQLiteCommand(query, connection);
                    insertCommandFilesTable.Parameters.AddWithValue("@Id", file.id);
                    insertCommandFilesTable.Parameters.AddWithValue("@ParentId", file.idParent);
                    insertCommandFilesTable.Parameters.AddWithValue("@Name", file.name);
                    insertCommandFilesTable.Parameters.AddWithValue("@Path", file.path);
                    insertCommandFilesTable.Parameters.AddWithValue("@LastUpdate", file.lastUpdate);
                    insertCommandFilesTable.Parameters.AddWithValue("@Version", file.version);
                    insertCommandFilesTable.Parameters.AddWithValue("@HashSum", file.hashSum);

                    insertCommandFilesTable.ExecuteNonQuery();
                }

                using var commandClearTableCheks = new SQLiteCommand("DELETE FROM CheksTable", connection);
                commandClearTableCheks.ExecuteNonQuery();

                string queryInfoAdd = "INSERT INTO CheksTable (ID, Creator, Date) " +
                                   "VALUES (@Id, @Creator, @Date);";

                using var insertCommandChecksTable = new SQLiteCommand(queryInfoAdd, connection);
                insertCommandChecksTable.Parameters.AddWithValue("@Id", 1);
                insertCommandChecksTable.Parameters.AddWithValue("@Creator", "Admin");
                insertCommandChecksTable.Parameters.AddWithValue("@Date", $"{DateTime.Now.ToString()}");
                insertCommandChecksTable.ExecuteNonQuery();
            }
        }

        public static FileTree GetEtalon()
        {
            var etalon = new List<FileDB>();

            using (var connection = new SQLiteConnection("Data Source=FileIntegrityDB.db"))
            {
                connection.Open();
                string query = "SELECT ID, ParentID, Name, Path, LastUpdate, Version, HashSum FROM FilesTable";

                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["ID"]);
                    int parentId = Convert.ToInt32(reader["ParentID"]);
                    string name = reader["Name"].ToString();
                    string path = reader["Path"].ToString();
                    string lastUpdate = reader["LastUpdate"].ToString();
                    string version = reader["Version"].ToString();
                    string hashSum = reader["HashSum"].ToString();

                    etalon.Add(new FileDB(id, name, path, lastUpdate, version, hashSum, parentId));
                }
            }

            var converter = new DataBaseConverter();
            var etalon1 = converter.ConvertFormatDBToFileTree(etalon);


            return etalon1;
        }

    }
}
