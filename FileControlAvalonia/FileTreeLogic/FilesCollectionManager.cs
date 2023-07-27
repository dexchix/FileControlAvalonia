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
        /// Добавление файлов в главное дерево 
        /// </summary>
        public static void AddFiles(FileTree mainFileTree, FileTree addedFileTree)
        {
            foreach (var file in addedFileTree.Children!.ToList())
            {
                if (!mainFileTree.Children!.Any(x => x.Path == file.Path))
                {
                    var mainParent = FileTreeNavigator.SearchFile(file.Parent!.Path, mainFileTree);
                    mainFileTree.Children!.Add(file);
                    //Добавление в БД
                    //=======================================================================
                    EtalonManager.AddFileInDB(file);
                    file.EVersion = file.FVersion;
                    file.EHash = file.FHash;
                    file.ELastUpdate = file.FLastUpdate;
                    //========================================================================

                    file.Parent = mainParent;
                }
                else if (mainFileTree.Children!.Any(x => x.Path == file.Path) && file.IsDirectory)
                {
                    AddFiles(mainFileTree.Children!.Where(x => x.Path == file.Path).FirstOrDefault()!, file);
                }
            }
        }
        /// <summary>
        /// Удаление файла из главного дерева и view-коллекции
        /// </summary>
        /// <param name="delitedFile"></param>
        /// <param name="viewCollectionFiles"></param>
        /// <param name="mainFileTree"></param>
        public static void DeliteFile(FileTree delitedFile, ObservableCollection<FileTree> viewCollectionFiles,
                                      FileTree mainFileTree)
        {
            try
            {
                foreach (var file in viewCollectionFiles.ToList())
                {
                    if (file.Path == delitedFile.Path)
                    {
                        viewCollectionFiles.Remove(file);
                        var delitedFileInFileTree = FileTreeNavigator.SearchFile(delitedFile.Path, mainFileTree);
                        delitedFileInFileTree.Parent!.Children!.Remove(delitedFileInFileTree);
                        return;
                    }
                }
                foreach (var file in viewCollectionFiles.ToList())
                {
                    var delFileInViewCollection = FileTreeNavigator.SearchFile(delitedFile.Path, file);
                    if (delFileInViewCollection != null)
                    {
                        delFileInViewCollection.Parent!.Children!.Remove(delFileInViewCollection);
                        var delitedFileInFileTree = FileTreeNavigator.SearchFile(delitedFile.Path, mainFileTree);
                        delitedFileInFileTree.Parent!.Children!.Remove(delitedFileInFileTree);
                        return;
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
        public static void UpdateViewFilesCollection(ObservableCollection<FileTree> viewCollectionFiles, FileTree mainFileTree)
        {
            viewCollectionFiles.Clear();
            foreach (var file in mainFileTree.Children!.ToList())
            {
                viewCollectionFiles?.Add(file);
            }
        }
        public static void MergeFileTrees(FileTree mainFileTree, FileTree etalonFileTree)
        {


            foreach (var file in mainFileTree.Children!.ToList())
            {
                var etalonFile = FileTreeNavigator.SearchFile(file.Path, etalonFileTree);
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
