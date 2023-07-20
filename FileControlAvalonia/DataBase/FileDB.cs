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

        //[PrimaryKey]
        //public int ID { get => id; set => id = value; }
        //public int ParentID { get => idParent; set => idParent = value; }
        //public string Name { get => name; set => name = value; }
        //public string Path { get => path; set => path = value; }
        //public string LastUpdate { get => lastUpdate; set => lastUpdate = value; }
        //public string Version { get => version; set => version = value; }
        //public string HashSum { get => hashSum; set => hashSum = value; }

        //public FileDB(int id, string name, string path, string lastUpdate, string version, string hashSum, int idParent =0)
        //{
        //    this.id = id;
        //    this.name = name;
        //    this.path = path;
        //    this.lastUpdate = lastUpdate;
        //    this.version = version;
        //    this.hashSum = hashSum;
        //    this.idParent = idParent;
        //}
        public FileDB()
        {

        }

        public FileDB(int iD, string name, string path, string lastUpdate, string version, string hashSum, int parentID = 0)
        {
            ID = iD;
            ParentID = parentID;
            Name = name;
            Path = path;
            LastUpdate = lastUpdate;
            Version = version;
            HashSum = hashSum;
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