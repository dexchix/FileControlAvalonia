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
        public string Name { get; set; }
        public string Path { get; set; }
        public string ELastUpdate { get; set; }
        public string EVersion { get; set; }
        public string EHashSum { get; set; }
        public string FLastUpdate { get; set; }
        public string FVersion { get; set; }
        public string FHashSum { get; set; }
        public string ParentPath { get; set; }
        public StatusFile Status { get; set; }
        public FileDB()
        {

        }

        public FileDB(string name, string path, string eLastUpdate, string eVersion, string eHashSum, string fLastUpdate, string fVersion, string fHashSum, string parentPath)
        {
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
    public class EtalonAndChecksInfoDB
    {
        public string Creator { get; set; }
        public string Date { get; set; }
        public string DateLastCheck { get; set; }
        public int TotalFiles { get; set; }
        public int Checked { get; set; }
        public int PartialChecked { get; set; }
        public int FailedChecked { get; set; }
        public int NoAccess { get; set; }
        public int NotFound { get; set; }
        public int NotChecked { get; set; }

        public EtalonAndChecksInfoDB()
        {

        }
    }
}
