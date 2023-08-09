using AutoMapper;
using FileControlAvalonia.Core;
using FileControlAvalonia.DataBase;
using FileControlAvalonia.Models;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileControlAvalonia.FileTreeLogic
{
    public static class FilesCollectionManager
    {
        /// <summary>
        /// Добавление файлов в существующее дерево 
        /// </summary>
        private static void AddFilesToExistingFileTree(FileTree mainFileTree, FileTree addedFileTree, ObservableCollection<FileTree> addedFilesCollection)
        {
            foreach (var file in addedFileTree.Children!.ToList())
            {
                if (!mainFileTree.Children!.Any(x => x.Path == file.Path))
                {
                    var mainParent = FileTreeNavigator.SearchFileInFileTree(file.Parent!.Path, mainFileTree);
                    mainFileTree.Children!.Add(file);
                    addedFilesCollection.Add(file);
                    file.Parent = mainParent;
                }
                else if (mainFileTree.Children!.Any(x => x.Path == file.Path) && file.IsDirectory)
                {
                    AddFilesToExistingFileTree(mainFileTree.Children!.Where(x => x.Path == file.Path).FirstOrDefault()!, file, addedFilesCollection);
                }
            }
        }

        public static void AddFiles(ObservableCollection<FileTree> mainCollectionFiles, ObservableCollection<FileTree> addedFiles, FileStats stats)
        {
            var addedBDFilesCollection = new ObservableCollection<FileTree>();
            foreach (var addFile in addedFiles)
            {
                if (mainCollectionFiles.Any(x => x.Path == addFile.Path) && addFile.IsDirectory)
                {
                    AddFilesToExistingFileTree(mainCollectionFiles.FirstOrDefault(x => x.Path == addFile.Path)!, addFile, addedBDFilesCollection);
                }
                else if (!mainCollectionFiles.Any(x => x.Path == addFile.Path))
                {
                    mainCollectionFiles.Add(addFile);
                    addedBDFilesCollection.Add(addFile);
                }
            }
            stats.GetFilesStats(addedBDFilesCollection);
            EtalonManager.AddFilesOrCreateEtalon(addedBDFilesCollection, false);
        }

        /// <summary>
        /// Удаление файла из главного дерева и view-коллекции
        /// </summary>
        /// <param name="delitedFile"></param>
        /// <param name="viewCollectionFiles"></param>
        /// <param name="mainFileTree"></param>
        public static void DeliteFile(FileTree delitedFile, ObservableCollection<FileTree> viewCollectionFiles,
                                      ObservableCollection<FileTree> mainFileTreeCollection, FileStats fileStats)
        {
            try
            {
                fileStats = fileStats.GetFilesStats(new List<FileTree> { delitedFile });

                var delitedFileMainCollection = FileTreeNavigator.SeachFileInFilesCollection(delitedFile.Path, mainFileTreeCollection);
                var delitedFileViewCollection = FileTreeNavigator.SeachFileInFilesCollection(delitedFile.Path, viewCollectionFiles);


                if (delitedFileMainCollection != null && delitedFileMainCollection.Parent != null) delitedFileMainCollection.Parent.Children!.Remove(delitedFileMainCollection);
                else
                {
                    var delFile = mainFileTreeCollection.Where(x => x.Path == delitedFile.Path).FirstOrDefault();
                    mainFileTreeCollection.Remove(delFile!);
                }

                
                if (delitedFileViewCollection != null && delitedFileViewCollection.Parent != null) delitedFileViewCollection.Parent.Children!.Remove(delitedFileViewCollection);
                else
                {
                    var delFile = viewCollectionFiles.Where(x => x.Path == delitedFile.Path).FirstOrDefault();
                    viewCollectionFiles.Remove(delFile!);
                }

            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Обновляет view коллекцию файлов
        /// </summary>
        /// <param name="viewCollectionFiles"></param>
        /// <param name="mainFileTree"></param>
        public static void UpdateViewFilesCollection(ObservableCollection<FileTree> viewCollectionFiles, ObservableCollection<FileTree> mainFileTreeCollectin)
        {
            viewCollectionFiles.Clear();
            foreach (var file in mainFileTreeCollectin.ToList())
            {
                viewCollectionFiles?.Add(file);
            }
        }
        /// <summary>
        /// Устанавливает эталонные значения в соответствии с фактическими (при добавлении файлов)
        /// </summary>
        /// <param name="filesCollection"></param>
        public static void SetEtalonValues(ObservableCollection<FileTree> filesCollection)
        {
            foreach (var file in filesCollection)
            {
                file.EVersion = file.FVersion;
                file.ELastUpdate = file.FLastUpdate;
                file.EHash = file.FHash;

                file.Status = StatusFile.Checked;

                if (file.IsDirectory)
                    SetEtalonValues(file.Children);
            }
        }

        public static ObservableCollection<FileTree> GetDeepCopyFilesCollection(ObservableCollection<FileTree> mainCollection)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FileTree, FileTree>();
            });

            var mapper = new Mapper(config);

            var clone = mapper.Map<ObservableCollection<FileTree>>(mainCollection);
            return clone;
        }


        public static FileTree GetDeepCopyFileTree(FileTree fileTree)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FileTree, FileTree>();
            });

            var mapper = new Mapper(config);

            var clone = mapper.Map<FileTree>(fileTree);
            return clone;
        }
        public static void FileTreeDestroction(FileTree fileTree)
        {
            var listDestroctionFiles = new List<FileTree>();
            listDestroctionFiles.Add(fileTree);




            void FillList(FileTree fileTree)
            {
                foreach(var file in fileTree.Children)
                {
                    listDestroctionFiles.Add(file);
                    if (file.IsDirectory) FillList(file);
                }
            }

            FillList(fileTree);


            foreach (var file in listDestroctionFiles)
            {

                if (file.Parent != null) file.Parent = null;
                if (file.Children != null)
                {
                    file.Children.Clear();
                }
            }
        }
    }
}
