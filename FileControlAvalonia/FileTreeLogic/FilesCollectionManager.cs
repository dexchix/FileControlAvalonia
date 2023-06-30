using FileControlAvalonia.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace FileControlAvalonia.FileTreeLogic
{
    public static class FilesCollectionManager
    {
        public static void AddFiles(this ObservableCollection<FileTree> mainCollection,
                                         ObservableCollection<FileTree> addedCollection)
        {
            foreach (var file in addedCollection)
            {
                if (!mainCollection.Any(x => x.Path == file.Path))
                {
                    mainCollection.Add(file);
                }
                else if (mainCollection.Any(x => x.Path == file.Path) && file.IsDirectory)
                {
                    AddFiles(mainCollection.Where(x => x.Path == file.Path).FirstOrDefault()!.Children!, file.Children!);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void AddFiles(FileTree mainFileTree, FileTree addedFileTree)
        {
            foreach (var file in addedFileTree.Children!.ToList())
            {
                if (!mainFileTree.Children!.Any(x => x.Path == file.Path))
                {
                    var mainParent = FileTreeNavigator.SearchFile(file.Parent!.Path, mainFileTree);
                    mainFileTree.Children!.Add(file);
                    file.Parent = mainParent;
                }
                else if (mainFileTree.Children!.Any(x => x.Path == file.Path) && file.IsDirectory)
                {
                    AddFiles(mainFileTree.Children!.Where(x => x.Path == file.Path).FirstOrDefault()!, file);
                }
            }
        }
        public static void DeliteFile(FileTree delitedFile, ObservableCollection<FileTree> viewCollectionFiles, 
                                      FileTree mainFileTree)
        {
            foreach (var file in viewCollectionFiles.ToList())
            {
                if (file.Path == delitedFile.Path)
                {
                    viewCollectionFiles.Remove(file);
                    var delitedFileInFileTree = FileTreeNavigator.SearchFile(delitedFile.Path, mainFileTree);
                    delitedFile.Parent.Children!.Remove(delitedFileInFileTree);
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
                    delitedFile.Parent.Children!.Remove(delitedFileInFileTree);
                    return;
                }
            }
        }
        public static void UpdateViewFilesCollection(ObservableCollection<FileTree> viewCollectionFiles, FileTree mainFileTree)
        {
            viewCollectionFiles.Clear();
            foreach (var file in mainFileTree.Children!.ToList())
            {
                viewCollectionFiles?.Add(file);
            }
        }
        public static FileTree CreateEmptyFileTree()
        {
            var fileTree = new FileTree(FileTreeNavigator.pathRootFolder, true);
            fileTree.Children!.Clear();
            return fileTree;

        }
    }
}
