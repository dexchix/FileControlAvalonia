using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.DataBase
{
    public class FileDB
    {
        public int id;
        public int idParent;
        public string name;
        public string path;
        public string lastUpdate;
        public string version;
        public string hashSum;

        public FileDB(int id, string name, string path, string lastUpdate, string version, string hashSum, int idParent =0)
        {
            this.id = id;
            this.name = name;
            this.path = path;
            this.lastUpdate = lastUpdate;
            this.version = version;
            this.hashSum = hashSum;
            this.idParent = idParent;
        }
    }
}

                                                //ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                 //ParentID INT,
                                                 //Name VARCHAR(512),
                                                 //Path VARCHAR(512),
                                                 //LustUpdate VARCHAR(512),
                                                 //HashSum VARCHAR(512)