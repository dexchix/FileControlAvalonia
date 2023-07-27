using FileControlAvalonia.FileTreeLogic;
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

        private int fileCounter = 0;
        private Dictionary<string, FileDB> parents = new Dictionary<string, FileDB>();
        public List<FileDB> ConvertFormatFileTreeToDB(FileTree mainFileTree)
        {
            var filesToDB = new List<FileDB>();
            FillDBListFiles(mainFileTree, filesToDB);
            AddParentsInDBFilesList(filesToDB);
            fileCounter = 0;
            return filesToDB;
        }
        public FileTree ConvertFormatDBToFileTree(List<FileDB> files)
        {

            var etalonTree = new FileTree(files[fileCounter].Path, true, isRoot: true)
            {
                ID = files[fileCounter].ID,
                EHash = files[fileCounter].HashSum,
                ELastUpdate = files[fileCounter].LastUpdate,
                EVersion = files[fileCounter].Version,
            };
            if (etalonTree.IsDirectory)
            {
                etalonTree.Children!.Clear();
            }
            fileCounter++;

            while (fileCounter < files.Count)
            {
                var addFile = new FileTree(files[fileCounter].Path, Directory.Exists(files[fileCounter].Path),
                                           FileTreeNavigator.FindObjectById(etalonTree, files[fileCounter].ParentID), loadChildren: false)
                {
                    ID = files[fileCounter].ID,
                    EHash = files[fileCounter].HashSum,
                    ELastUpdate = files[fileCounter].LastUpdate,
                    EVersion = files[fileCounter].Version,
                };
                if (addFile.IsDirectory)
                {
                    addFile.Children.Clear();
                }
                var parent = FileTreeNavigator.SearchFile(Path.GetDirectoryName(addFile.Path)!,etalonTree);
                parent.Children!.Add(addFile);
                fileCounter++;
            }
            return etalonTree;
        }

        public void FillDBListFiles(FileTree mainFileTree, List<FileDB> filesDB)
        {
            if (filesDB.Count == 0)
            {
                var addedFileDB = new FileDB(fileCounter, mainFileTree.Name, mainFileTree.Path, mainFileTree.FLastUpdate, mainFileTree.FVersion, mainFileTree.FHash);
                //================================================================
                mainFileTree.EHash = mainFileTree.FHash;
                mainFileTree.ELastUpdate = mainFileTree.FLastUpdate;
                mainFileTree.EVersion = mainFileTree.FVersion;
                mainFileTree.Status = Core.StatusFile.Checked;
                //================================================================

                filesDB.Add(addedFileDB);
                parents.Add(addedFileDB.Path, addedFileDB);
                fileCounter++;
            }
            foreach (var file in mainFileTree.Children!.ToList())
            {

                if (file.IsDirectory)
                {
                    var addedFileDB = new FileDB(fileCounter, file.Name, file.Path, file.FLastUpdate, file.FVersion, file.FHash);
                    filesDB.Add(addedFileDB);
                    parents.Add(addedFileDB.Path, addedFileDB);
                    fileCounter++;
                    FillDBListFiles(file, filesDB);


                    //================================================================
                    file.EHash = file.FHash;
                    file.ELastUpdate = file.FLastUpdate;
                    file.EVersion = file.FVersion;
                    file.Status = Core.StatusFile.Checked;
                    //================================================================
                }
                else
                {
                    filesDB.Add(new FileDB(fileCounter, file.Name, file.Path, file.FLastUpdate, file.FVersion, file.FHash));
                    fileCounter++;

                    //================================================================
                    file.EHash = file.FHash;
                    file.ELastUpdate = file.FLastUpdate;
                    file.EVersion = file.FVersion;
                    file.Status = Core.StatusFile.Checked;
                    //================================================================
                }
            }
        }
        private void AddParentsInDBFilesList(List<FileDB> filesDB)
        {
            foreach (var file in filesDB)
            {
                var pathParent = Path.GetDirectoryName(file.Path);
                foreach (var parent in parents)
                {
                    if (pathParent == parent.Key)
                    {
                        file.ParentID = parent.Value.ID;
                    }
                }
            }
        }
    }
}
