using FileControlAvalonia.Core;
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
        public string ELastUpdate { get; set; }
        public string EVersion { get; set; }
        public string EHashSum { get; set; }
        public string FLastUpdate { get; set; }
        public string FVersion { get; set; }
        public string FHashSum { get; set; }
        public string ParentPath { get; set; }
        public StatusFile StatusFile { get; set; }
        public FileDB()
        {

        }

        public FileDB(int iD, string name, string path, string eLastUpdate, string eVersion, string eHashSum, string fLastUpdate, string fVersion, string fHashSum, string parentPath,  int parentID = 0)
        {
            ID = iD;
            ParentID = parentID;
            Name = name;
            Path = path;
            ELastUpdate = eLastUpdate;
            EVersion = eVersion;
            EHashSum = eHashSum;
            FLastUpdate = fLastUpdate;
            FVersion = fVersion;
            FHashSum = fHashSum;

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