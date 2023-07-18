using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
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

            var etalonTree = new FileTree(files[fileCounter].path, true, isRoot: true, loadChildren: false)
            {
                ID = files[fileCounter].id,
                EHash = files[fileCounter].hashSum,
                ELastUpdate = files[fileCounter].lastUpdate,
                EVersion = files[fileCounter].version,
            };
            if (etalonTree.IsDirectory)
            {
                etalonTree.Children.Clear();
            }
            fileCounter++;

            while (fileCounter < files.Count)
            {
                var addFile = new FileTree(files[fileCounter].path, Directory.Exists(files[fileCounter].path),
                                           FindObjectById(etalonTree, files[fileCounter].idParent), loadChildren: false)
                {
                    ID = files[fileCounter].id,
                    EHash = files[fileCounter].hashSum,
                    ELastUpdate = files[fileCounter].lastUpdate,
                    EVersion = files[fileCounter].version,
                };
                if (addFile.IsDirectory)
                {
                    addFile.Children.Clear();
                }
                var parent = FindObjectById(etalonTree, files[fileCounter].idParent);
                    parent.Children.Add(addFile);
                fileCounter++;
            }
            return etalonTree;
        }

        public FileTree FindObjectById(FileTree etalonFileTree, int targetId)
        {
            if (etalonFileTree.ID == targetId)
                return etalonFileTree;

            foreach (var child in etalonFileTree.Children.ToList())
            {
                if(child.IsDirectory)
                {
                    var result = FindObjectById(child, targetId);
                    if (result != null)
                        return result;
                }
            }
            return null;
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
                parents.Add(addedFileDB.path, addedFileDB);
                fileCounter++;
            }
            foreach (var file in mainFileTree.Children!)
            {

                if (file.IsDirectory)
                {
                    var addedFileDB = new FileDB(fileCounter, file.Name, file.Path, file.FLastUpdate, file.FVersion, file.FHash);
                    filesDB.Add(addedFileDB);
                    //if (parents.All(x => x.Key == addedFileDB.path))
                    parents.Add(addedFileDB.path, addedFileDB);
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
                var pathParent = Path.GetDirectoryName(file.path);
                foreach (var parent in parents)
                {
                    if (pathParent == parent.Key)
                    {
                        file.idParent = parent.Value.id;
                    }
                }
            }
        }
    }
}
