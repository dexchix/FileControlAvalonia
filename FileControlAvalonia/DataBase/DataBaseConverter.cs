using Avalonia.OpenGL;
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
        public List<FileDB> ConvertFormatFileTreeToDB(ObservableCollection<FileTree> mainFileTreeCollection)
        {
            var filesToDB = new List<FileDB>();
            FillDBListFiles(mainFileTreeCollection, filesToDB);
            AddParentsInDBFilesList(filesToDB);
            fileCounter = 0;
            return filesToDB;
        }
        public ObservableCollection<FileTree> ConvertFormatDBToFileTreeCollection(List<FileDB> files)
        {
            var etalon = new ObservableCollection<FileTree>();

            while (fileCounter < files.Count)
            {
                if (files[fileCounter].ParentPath == "")
                {
                    var addFile = new FileTree(files[fileCounter].Path, Directory.Exists(files[fileCounter].Path))
                    {
                        ID = files[fileCounter].ID,
                        EHash = files[fileCounter].HashSum,
                        ELastUpdate = files[fileCounter].LastUpdate,
                        EVersion = files[fileCounter].Version,

                        FHash = files[fileCounter].HashSum,
                        FLastUpdate = files[fileCounter].LastUpdate,
                        FVersion = files[fileCounter].Version
                    };
                    
                    if (addFile.IsDirectory)
                        addFile.Children!.Clear();
                    etalon.Add(addFile);
                    fileCounter++;
                }
                else
                {
                    var addFile = new FileTree(files[fileCounter].Path, Directory.Exists(files[fileCounter].Path),
                                               FileTreeNavigator.SeachFileInFilesCollection(files[fileCounter].ParentPath, etalon))
                    {
                        ID = files[fileCounter].ID,
                        EHash = files[fileCounter].HashSum,
                        ELastUpdate = files[fileCounter].LastUpdate,
                        EVersion = files[fileCounter].Version,

                        FHash = files[fileCounter].HashSum,
                        FLastUpdate = files[fileCounter].LastUpdate,
                        FVersion = files[fileCounter].Version
                    };
                    if (addFile.IsDirectory)
                    {
                        addFile.Children.Clear();
                    }
                    var parent = FileTreeNavigator.SeachFileInFilesCollection(Path.GetDirectoryName(addFile.Path)!, etalon);
                    parent.Children!.Add(addFile);
                    fileCounter++;
                }
            }
            return etalon;
        }

        public void FillDBListFiles(ObservableCollection<FileTree> mainFileTreeCollection, List<FileDB> filesDB)
        {
            foreach (var file in mainFileTreeCollection.ToList())
            {

                if (file.IsDirectory)
                {
                    FileDB addedFileDB;
                    if (file.Parent == null)
                        addedFileDB = new FileDB(fileCounter, file.Name, file.Path, file.FLastUpdate, file.FVersion, file.FHash, null);
                    else
                        addedFileDB = new FileDB(fileCounter, file.Name, file.Path, file.FLastUpdate, file.FVersion, file.FHash, file.Parent.Path);
                    filesDB.Add(addedFileDB);
                    parents.Add(addedFileDB.Path, addedFileDB);
                    fileCounter++;
                    FillDBListFiles(file.Children, filesDB);


                    //================================================================
                    file.EHash = file.FHash;
                    file.ELastUpdate = file.FLastUpdate;
                    file.EVersion = file.FVersion;
                    file.Status = Core.StatusFile.Checked;
                    //================================================================
                }
                else
                {
                    FileDB addedFileDB;
                    if (file.Parent == null)
                        addedFileDB = new FileDB(fileCounter, file.Name, file.Path, file.FLastUpdate, file.FVersion, file.FHash, null);
                    else
                        addedFileDB = new FileDB(fileCounter, file.Name, file.Path, file.FLastUpdate, file.FVersion, file.FHash, file.Parent.Path);
                    filesDB.Add(addedFileDB);
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
