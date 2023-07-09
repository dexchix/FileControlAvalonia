using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus;

namespace FileControlAvalonia.Core
{
    public class CheksInfoManager
    {
        public static void RecordDataOfLastCheck(string dateLastCheck)
        {
            using (var connection = new SQLiteConnection("Data Source=FileIntegrityDB.db"))
            {
                connection.Open();
                string query = "UPDATE CheksTable SET DateLastCheck = @DateLastCheck";
                using var insertInfoCommand = new SQLiteCommand(query, connection);
                insertInfoCommand.Parameters.AddWithValue("@DateLastCheck", dateLastCheck);
                insertInfoCommand.ExecuteNonQuery();
            }
        }
        public static void RecordInfoOfCreateEtalon(string userLevel, string dateCreateEtalon)
        {
            using (var connection = new SQLiteConnection("Data Source=FileIntegrityDB.db"))
            {
                connection.Open();
                string query = "UPDATE CheksTable SET Creator = @Creator, Date = @Date";
                using var insertInfoCommand = new SQLiteCommand(query, connection);
                insertInfoCommand.Parameters.AddWithValue("@Creator", userLevel);
                insertInfoCommand.Parameters.AddWithValue("@Date", dateCreateEtalon);
                insertInfoCommand.ExecuteNonQuery();
            }
        }
        public static void GetDataOfCreateEtalon()
        {

        }
        public static void GetInfoOfCreateEtalon()
        {

        }
    }
}
