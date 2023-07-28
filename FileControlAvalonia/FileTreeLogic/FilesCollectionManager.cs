using FileControlAvalonia.Core;
using FileControlAvalonia.DataBase;
using FileControlAvalonia.Models;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileControlAvalonia.FileTreeLogic
{
    public static class FilesCollectionManager
    {
        /// <summary>
        /// Добавление файлов в существующее дерево 
        /// </summary>
        private static void AddFilesToExistingFileTree(FileTree mainFileTree, FileTree addedFileTree)
        {
            var addedFilesCollection = new ObservableCollection<FileTree>();
            foreach (var file in addedFileTree.Children!.ToList())
            {
                if (!mainFileTree.Children!.Any(x => x.Path == file.Path))
                {
                    var mainParent = FileTreeNavigator.SearchFileInFileTree(file.Parent!.Path, mainFileTree);
                    mainFileTree.Children!.Add(file);
                    //Добавление в БД
                    //=======================================================================
                    addedFilesCollection.Add(file);
                    file.EVersion = file.FVersion;
                    file.EHash = file.FHash;
                    file.ELastUpdate = file.FLastUpdate;
                    //========================================================================

                    file.Parent = mainParent;
                }
                else if (mainFileTree.Children!.Any(x => x.Path == file.Path) && file.IsDirectory)
                {
                    AddFilesToExistingFileTree(mainFileTree.Children!.Where(x => x.Path == file.Path).FirstOrDefault()!, file);
                }
            }
            EtalonManager.CreateEtalon(addedFilesCollection);
        }
        public static void AddFiles(ObservableCollection<FileTree> mainCollectionFiles, ObservableCollection<FileTree> addedFiles)
        {
            foreach(var addFile in addedFiles)
            {
                if(mainCollectionFiles.Any(x=>x.Path == addFile.Path) && addFile.IsDirectory)
                {
                    AddFilesToExistingFileTree(mainCollectionFiles.FirstOrDefault(x=>x.Path == addFile.Path)!, addFile);
                }
                else if (!mainCollectionFiles.Any(x => x.Path == addFile.Path))
                {
                    mainCollectionFiles.Add(addFile);
                    //Добавление в БД
                    //=======================================================================
                    //EtalonManager.AddFileInDB(addFile);
                    addFile.EVersion = addFile.FVersion;
                    addFile.EHash = addFile.FHash;
                    addFile.ELastUpdate = addFile.FLastUpdate;
                    //=======================================================================
                }
            }
            EtalonManager.CreateEtalon(addedFiles);
        }

        /// <summary>
        /// Удаление файла из главного дерева и view-коллекции
        /// </summary>
        /// <param name="delitedFile"></param>
        /// <param name="viewCollectionFiles"></param>
        /// <param name="mainFileTree"></param>
        public static void DeliteFile(FileTree delitedFile, ObservableCollection<FileTree> viewCollectionFiles,
                                      ObservableCollection<FileTree> mainFileTreeCollection)
        {
            try
            {
                foreach (var file in viewCollectionFiles.ToList())
                {
                    if (file.Path == delitedFile.Path)
                    {
                        viewCollectionFiles.Remove(file);
                        mainFileTreeCollection.Remove(file);
                        return;
                    }
                }
                foreach (var file in viewCollectionFiles.ToList())
                {
                    if (delitedFile.Path.StartsWith(file.Path))
                    {
                        var delFileInViewCollection = FileTreeNavigator.SearchFileInFileTree(delitedFile.Path, file);
                        if (delFileInViewCollection != null)
                        {
                            delFileInViewCollection.Parent!.Children!.Remove(delFileInViewCollection);
                            var delitedFileInFileTree = FileTreeNavigator.SeachFileInFilesCollection(delitedFile.Path, mainFileTreeCollection);
                            delitedFileInFileTree.Parent!.Children!.Remove(delitedFileInFileTree);
                            return;
                        }
                    }
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
        public static void MergeFileTrees(FileTree mainFileTree, FileTree etalonFileTree)
        {


            foreach (var file in mainFileTree.Children!.ToList())
            {
                var etalonFile = FileTreeNavigator.SearchFileInFileTree(file.Path, etalonFileTree);
                if (etalonFile != null)
                {

                    file.EVersion = etalonFile.EVersion;
                    file.EHash = etalonFile.EHash;
                    file.ELastUpdate = etalonFile.ELastUpdate;

                    if (file.IsDirectory)
                    {
                        MergeFileTrees(file, etalonFile);
                    }
                }
            }
        }
    }
}
