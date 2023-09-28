using FileControlAvalonia.Core;
using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileControlAvalonia.DataBase
{
    public class DataBaseConverter
    {
        private object _lock = new object();
        private int _count = 0;
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
            var startTime = DateTime.Now;
            var etalon = new ObservableCollection<FileTree>();
            var filesDictionary = new Dictionary<string, FileTree>();

            var rootsDictionary = new List<FileTree>();

            if (files.Count <= 1000)
            {
                while (fileCounter < files.Count)
                {
                    if (files[fileCounter].ParentPath == string.Empty)
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
            else
            {
                int start = 0;
                int limit = 1;
                var section = files.Count / 8;
                int residue = files.Count - section * 8;
                for (int i = 0; i < 8; i++)
                {
                    int localStart = start * section;
                    int localLimit = limit * section;
                    start++;
                    limit++;
                    Task.Run(() =>
                    {
                        for (int i = localStart; i < localLimit; i++)
                        {
                            var addFile = new FileTree(files[i].Path, files[i].IsDirectory, false)
                            {
                                EHash = files[i].EHashSum,
                                ELastUpdate = files[i].ELastUpdate,
                                EVersion = files[i].EVersion,

                                FHash = files[i].FHashSum,
                                FLastUpdate = files[i].FLastUpdate,
                                FVersion = files[i].FVersion,

                                Status = files[i].Status,
                            };
                            lock (_lock)
                            {
                                filesDictionary.Add(addFile.Path, addFile);
                                if (files[i].ParentPath == string.Empty)
                                    rootsDictionary.Add(addFile);
                                _count++;
                            }
                        }
                    });
                }
                for (int i = files.Count - residue; i < files.Count; i++)
                {
                    var addFile = new FileTree(files[i].Path, files[i].IsDirectory, false)
                    {
                        EHash = files[i].EHashSum,
                        ELastUpdate = files[i].ELastUpdate,
                        EVersion = files[i].EVersion,

                        FHash = files[i].FHashSum,
                        FLastUpdate = files[i].FLastUpdate,
                        FVersion = files[i].FVersion,

                        Status = files[i].Status,
                    };
                    lock (_lock)
                    {
                        filesDictionary.Add(addFile.Path, addFile);
                        if (files[i].ParentPath == string.Empty)
                            rootsDictionary.Add(addFile);
                        _count++;
                    }
                }

                while (true)
                {
                    if (_count == files.Count)
                    {
                        _count = 0;
                        start = 0;
                        limit = 1;
                        break;
                    }
                }
                //Присваивание Parent и Children
                for (int i = 0; i < 8; i++)
                {
                    int localStart = start * section;
                    int localLimit = limit * section;
                    start++;
                    limit++;
                    Task.Run(() =>
                    {
                        for (int i = localStart; i < localLimit; i++)
                        {
                            lock (_lock)
                            {
                                var file = filesDictionary[files[i].Path];
                                if (files[i].ParentPath != string.Empty)
                                {
                                    var fileParent = filesDictionary[files[i].ParentPath];
                                    if (fileParent != null)
                                    {
                                        file.Parent = fileParent;
                                        fileParent.Children.Add(file);
                                    }
                                }
                                _count++;
                            }
                        }
                    });
                }
                for (int i = files.Count - residue; i < files.Count; i++)
                {
                    lock (_lock)
                    {
                        var file = filesDictionary[files[i].Path];
                        if (files[i].ParentPath != string.Empty)
                        {
                            var fileParent = filesDictionary[files[i].ParentPath];
                            if (fileParent != null)
                            {
                                file.Parent = fileParent;
                                fileParent.Children.Add(file);
                            }
                        }
                        _count++;
                    }
                }
                while (true)
                {
                    if (_count == files.Count)
                    {
                        _count = 0;
                        break;
                    }
                }

                foreach (var file in rootsDictionary)
                {
                    etalon.Add(file);
                }
                var end = DateTime.Now;
                return etalon;
            }

        }

        public void FillDBListFiles(ObservableCollection<FileTree> mainFileTreeCollection, List<FileDB> filesDB)
        {
            foreach (var file in mainFileTreeCollection.ToList())
            {

                if (file.IsDirectory)
                {
                    FileDB addedFileDB;
                    if (file.Parent == null)
                        addedFileDB = new FileDB(file.Name, file.Path, file.ELastUpdate, file.EVersion, file.EHash, file.FLastUpdate, file.FVersion, file.FHash, null, file.IsDirectory) { Status = file.Status };
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
