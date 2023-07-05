using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.DataBase
{
    public class DataBaseConverter
    {
       
        private int countFiles = 0;
        private Dictionary<string, FileDB> parents = new Dictionary<string, FileDB>();  
        public List<FileDB> ConvertFileTreeToDBFormat(FileTree mainFileTree)
        {
            var filesToDB = new List<FileDB>();
            FillDBListFiles(mainFileTree, filesToDB);
            AddParentsInDBFilesList(filesToDB);  
            return filesToDB;
        }
        public ObservableCollection<FileTree> ConvertDBFormatToFileTree(List<FileDB> files)
        {
            return null;
        }
        public void FillDBListFiles(FileTree mainFileTree, List<FileDB> filesDB)
        {
            foreach (var file in mainFileTree.Children!)
            {

                if (file.IsDirectory)
                {
                    var addedFileDB = new FileDB(countFiles, file.Name, file.Path, file.FLastUpdate, file.FVersion, file.FHash);
                    filesDB.Add(addedFileDB);
                    parents.Add(addedFileDB.path, addedFileDB);
                    countFiles++;
                    FillDBListFiles(file, filesDB);
                }
                else
                {
                    filesDB.Add(new FileDB(countFiles, file.Name, file.Path, file.FLastUpdate, file.FVersion, file.FHash));
                    countFiles++;
                }
            }
        }
        private void AddParentsInDBFilesList(List<FileDB> filesDB)
        {
            foreach(var file in filesDB)
            {
                var pathParent = Path.GetDirectoryName(file.path);
                foreach(var parent in parents)
                {
                    if(pathParent == parent.Key)
                    {
                        file.idParent = parent.Value.id;
                    }
                }
            }
        }
    }
}
