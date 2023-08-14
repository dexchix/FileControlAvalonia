using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FileControlAvalonia.DataBase
{
    public class DataBaseConverter
    {

        private int fileCounter = 0;
        public List<FileDB> ConvertFormatFileTreeToDB(ObservableCollection<FileTree> mainFileTreeCollection)
        {
            var filesToDB = new List<FileDB>();
            FillDBListFiles(mainFileTreeCollection, filesToDB);
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
                    var addFile = new FileTree(files[fileCounter].Path, files[fileCounter].IsDirectory, false)
                    {
                        EHash = files[fileCounter].EHashSum,
                        ELastUpdate = files[fileCounter].ELastUpdate,
                        EVersion = files[fileCounter].EVersion,

                        FHash = files[fileCounter].FHashSum,
                        FLastUpdate = files[fileCounter].FLastUpdate,
                        FVersion = files[fileCounter].FVersion,

                        Status = files[fileCounter].Status,
                    };

                    etalon.Add(addFile);
                    fileCounter++;
                }
                else
                {
                    var addFile = new FileTree(files[fileCounter].Path, files[fileCounter].IsDirectory, false, 
                                               FileTreeNavigator.SeachFileInFilesCollection(files[fileCounter].ParentPath, etalon))
                    {
                        EHash = files[fileCounter].EHashSum,
                        ELastUpdate = files[fileCounter].ELastUpdate,
                        EVersion = files[fileCounter].EVersion,

                        FHash = files[fileCounter].FHashSum,
                        FLastUpdate = files[fileCounter].FLastUpdate,
                        FVersion = files[fileCounter].FVersion,

                        Status = files[fileCounter].Status,
                    };

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
                        addedFileDB = new FileDB(file.Name, file.Path, file.ELastUpdate, file.EVersion, file.EHash, file.FLastUpdate, file.FVersion, file.FHash, null, file.IsDirectory) { Status = file.Status};
                    else
                        addedFileDB = new FileDB(file.Name, file.Path, file.ELastUpdate, file.EVersion, file.EHash, file.FLastUpdate, file.FVersion, file.FHash, file.Parent.Path, file.IsDirectory) { Status = file.Status };
                    filesDB.Add(addedFileDB);

                    FillDBListFiles(file.Children, filesDB);
                }
                else
                {
                    FileDB addedFileDB;
                    if (file.Parent == null)
                        addedFileDB = new FileDB(file.Name, file.Path, file.ELastUpdate, file.EVersion, file.EHash, file.FLastUpdate, file.FVersion, file.FHash, null, file.IsDirectory) { Status = file.Status };
                    else
                        addedFileDB = new FileDB(file.Name, file.Path, file.ELastUpdate, file.EVersion, file.EHash, file.FLastUpdate, file.FVersion, file.FHash, file.Parent.Path, file.IsDirectory) { Status = file.Status };
                    filesDB.Add(addedFileDB);
                }
            }
        }
    }
}
