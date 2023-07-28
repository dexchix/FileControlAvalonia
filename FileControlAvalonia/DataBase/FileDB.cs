using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FileControlAvalonia.DataBase
{
    public class FileDB
    {

        [PrimaryKey]
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string LastUpdate { get; set; }
        public string Version { get; set; }
        public string HashSum { get; set; }
        public string ParentPath { get; set; }
        public FileDB()
        {

        }

        public FileDB(int iD, string name, string path, string lastUpdate, string version, string hashSum, string parentPath, int parentID = 0)
        {
            ID = iD;
            ParentID = parentID;
            Name = name;
            Path = path;
            LastUpdate = lastUpdate;
            Version = version;
            HashSum = hashSum;
            ParentPath = parentPath;
        }
        public override string ToString()
        {
            return Path;
        }
    }
}

//ID INTEGER PRIMARY KEY AUTOINCREMENT,
//ParentID INT,
//Name VARCHAR(512),
//Path VARCHAR(512),
//LustUpdate VARCHAR(512),
//HashSum VARCHAR(512)