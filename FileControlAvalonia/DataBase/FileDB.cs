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
        public string lustUpdate;
        public string hashSum;
    }
}

                                                 //ID INTEGER PRIMARY KEY AUTOINCREMENT,
                                                 //ParentID INT,
                                                 //Name VARCHAR(512),
                                                 //Path VARCHAR(512),
                                                 //LustUpdate VARCHAR(512),
                                                 //HashSum VARCHAR(512)