using FileControlAvalonia.DataBase;
using SQLite;
using System;
using System.Collections.Generic;
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
            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                var insertInfoCommand = new SQLiteCommand(connection)
                {
                    CommandText = $"UPDATE CheksTable SET DateLastCheck = '{dateLastCheck}'"
                };
                insertInfoCommand.ExecuteNonQuery();
            }
        }
        public static void RecordInfoOfCreateEtalon(string userLevel, string dateCreateEtalon)
        {
            using (var connection = new SQLiteConnection(DataBaseOptions.Options))
            {
                var insertInfoCommand = new SQLiteCommand(connection)
                {
                    CommandText = $"UPDATE CheksTable SET Creator = '{userLevel}', Date = '{dateCreateEtalon}'"
                };
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
