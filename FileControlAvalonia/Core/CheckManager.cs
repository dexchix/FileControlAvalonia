using FileControlAvalonia.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public class CheckManager
    {
        public static void CreateEtalon(List<FileDB> etalonFiles)
        {
            using (var connection = new SQLiteConnection("Data Source=FileIntegrityDB.db"))
            {
                connection.Open();
                foreach (var file in etalonFiles)
                {
                    string query = "INSERT INTO FilesTable (ID, ParentID, Name, Path, LastUpdate, Version, HashSum) " +
                                   "VALUES (@Id, @ParentId, @Name, @Path, @LastUpdate, @Version, @HashSum);";

                    using var insertCommand = new SQLiteCommand(query, connection);
                    insertCommand.Parameters.AddWithValue("@Id", file.id);
                    insertCommand.Parameters.AddWithValue("@ParentId", file.idParent);
                    insertCommand.Parameters.AddWithValue("@Name", file.name);
                    insertCommand.Parameters.AddWithValue("@Path", file.path);
                    insertCommand.Parameters.AddWithValue("@LastUpdate", file.lastUpdate);
                    insertCommand.Parameters.AddWithValue("@Version", file.version);
                    insertCommand.Parameters.AddWithValue("@HashSum", file.hashSum);

                    insertCommand.ExecuteNonQuery();
                }
            }
        }
        public static List<FileDB> GetEtalon()
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

            return etalon;
        }
    }
}
//private static void ConnectionDataBase()
//{
//    using (var connection = new SQLiteConnection("Data Source=FileIntegrityDB.db"))
//    {
//        connection.Open();

//        string createFilesTableQuery = @"CREATE TABLE IF NOT EXISTS FilesTable (
//                                                 ID INTEGER PRIMARY KEY AUTOINCREMENT,
//                                                 ParentID INT,
//                                                 Name VARCHAR(512),
//                                                 Path VARCHAR(512),
//                                                 LustUpdate VARCHAR(512),
//                                                 HashSum VARCHAR(512)
//                                             );";

//        string createCheksTableQuery = @"CREATE TABLE IF NOT EXISTS CheksTable (
//                                                 ID INTEGER PRIMARY KEY AUTOINCREMENT,
//                                                 Creator VARCHAR(512),
//                                                 Date VARCHAR(512)
//                                             );";

//        using (var command = new SQLiteCommand(connection))
//        {
//            command.CommandText = createFilesTableQuery;
//            command.ExecuteNonQuery();
//            command.CommandText = createCheksTableQuery;
//            command.ExecuteNonQuery();
//        }
//    }
//}
