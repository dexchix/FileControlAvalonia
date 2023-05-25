using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileControlAvalonia.Models
{
    public class FileTreeNavigator : ReactiveObject
    {
        #region FIELDS
        public readonly string _pathRootFolder = "C:\\1\\2";
        //public readonly string _pathRootFolder = "/home/orpo/Desktop/1/2";
        public static FileTree? _fileTree;
        public static Watcher? _watcher;
        new public event PropertyChangedEventHandler? PropertyChanged;
        #endregion 

        public FileTree FileTree
        {
            get => _fileTree!;
            set
            {
                _fileTree = value;
                OnPropertyChanged(nameof(FileTree));
            }
        }
        public FileTreeNavigator()
        {
            if (Directory.Exists(_pathRootFolder))
            {
                _fileTree = new FileTree(_pathRootFolder, true);
                _watcher = new Watcher(_pathRootFolder, this);
                CheckChangeRootPath();
            }
        }

        #region METHODS
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Переход в выбранную папку
        /// </summary>
        /// <param name="selectedFile"></param>
        public void GoToFolder(FileTree selectedFile)
        {
            if (selectedFile != null && Directory.Exists(selectedFile.Path))
            {
                FileTree = selectedFile;
            }
        }
        /// <summary>
        /// Возврат в родительскую папку
        /// </summary>
        public void GoBackFolder()
        {
            if (_fileTree != null && _fileTree.Parent != null)
            {
                FileTree = FileTree.Parent!;
            }
        }
        /// <summary>
        /// Поиск файла в дереве
        /// </summary>
        /// <param name="searchedFilePath"></param>
        /// <returns>Экземпляр типа FileTree (Файл)</returns>
        public FileTree SearchFile(string searchedFilePath)
        {
            if (FileTree.Path == searchedFilePath)
                return FileTree;
            else if (searchedFilePath.StartsWith(FileTree.Path))
                return SearchChildren(searchedFilePath, FileTree)!;
            else if (FileTree.Path.StartsWith(searchedFilePath))
                return SearchTreeParent(searchedFilePath, FileTree);
            else
                return SearchChildren(searchedFilePath, SearchTreeParent(searchedFilePath, FileTree))!;
        }
        /// <summary>
        /// Поиск родительского элемента в дереве
        /// </summary>
        /// <param name="searchedFilePath"></param>
        /// <param name="openedFolder"></param>
        /// <returns>Элемент типа FileTree (Файл)</returns>
        public FileTree SearchTreeParent(string searchedFilePath, FileTree openedFolder)
        {
            return searchedFilePath.StartsWith(openedFolder.Path)
                ? openedFolder
                : SearchTreeParent(searchedFilePath, openedFolder.Parent!);
        }
        /// <summary>
        /// Поиск дочернего элементав дереве
        /// </summary>
        /// <param name="searchedFilePath"></param>
        /// <param name="rootFolder"></param>
        /// <returns>Элемент типа FileTree (Файл)</returns>
        public FileTree? SearchChildren(string searchedFilePath, FileTree rootFolder)
        {
            var maxMatchFile = rootFolder.Children!.Where(x => searchedFilePath.StartsWith(x.Path))
                                                  .FirstOrDefault()!;
            try
            {
                return maxMatchFile.Path == searchedFilePath
                    ? maxMatchFile
                    : SearchChildren(searchedFilePath, maxMatchFile);
            }
            catch (Exception ex)
            {
                Program.logger.Error($"Не удалось найти файл в дереве {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Изменение путей дочерних элементов при изменении переименовании родительской папки
        /// </summary>
        /// <param name="changedFile"></param>
        public void ChangePathChildren(FileTree changedFile)
        {
            foreach (var child in changedFile.Children!.ToList())
            {
                child.Path = Path.Combine(changedFile.Path, child.Name);
                if (Directory.Exists(child.Path))
                {
                    ChangePathChildren(child);
                }
            }
        }
        /// <summary>
        /// Очистка формы 
        /// </summary>
        public void CheckExistRootPath()
        {
            try
            {
                FileTree.Path = "Исходный путь отсутствует!";
                FileTree.Children!.Clear();
                FileTree.Parent = null;
            }
            catch (Exception ex)
            {
                Program.logger.Error($"Ошибка отчиски формы. {ex}");
            }
        }
        /// <summary>
        /// Проверка изменения путей родительской папки и до неё
        /// </summary>
        private void CheckChangeRootPath()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (!Directory.Exists(_pathRootFolder))
                    {
                        CheckExistRootPath();
                        return;
                    }
                }
            });
        }
        //public ObservableCollection<FileTree>? selectedFiles = new ObservableCollection<FileTree>();
        //public void FillingCollectionSelectedItems(ObservableCollection<FileTree> fileTrees)
        //{
        //    foreach (var fileTree in fileTrees)
        //    {
        //        //fileTree.IsChecked == true ?
        //        if (fileTree.IsChecked == true)
        //        {
        //            selectedFiles!.Add(fileTree);
        //        }
        //        if (fileTree.IsDirectory && fileTree.IsChecked != true)
        //            FillingCollectionSelectedItems(fileTree.Children!);
        //    }
        //}

        public ObservableCollection<FileTree> GetSelectedFiles()
        {
            bool presenceEmptyFolders = true;
            var selectedFilesTree = SearchTreeParent(_pathRootFolder, FileTree).Clone() as FileTree;
            RemoveUnSelectedFiles(selectedFilesTree.Children!);

            //while (presenceEmptyFolders == true)
            //{
            //    RemoveEmptyFolders(selectedFilesTree.Children!);
            //    presenceEmptyFolders = CheckEmptyFolders(selectedFilesTree.Children!);
            //}
            //RemoveEmptyFolders(selectedFilesTree.Children!);
            return selectedFilesTree.Children!;
        }
        public void RemoveUnSelectedFiles(ObservableCollection<FileTree> folder)
        {
            foreach (var file in folder.ToList())
            {
                if (file.IsChecked == false && !file.IsDirectory)
                {
                    folder.Remove(file);
                }
                if (file.IsDirectory)
                {
                    RemoveUnSelectedFiles(file.Children!);
                }
            }
        }
        private void RemoveEmptyFolders(ObservableCollection<FileTree> selectedFiles)
        {
            foreach (var file in selectedFiles.ToList())
            {
                if (file.IsDirectory && file.Children.Count == 0)
                {
                    selectedFiles.Remove(file);
                }
                else if (file.IsDirectory && file.Children.Count != 0)
                {
                    RemoveEmptyFolders(file.Children);
                }
            }
        }

        private bool CheckEmptyFolders(ObservableCollection<FileTree> files)
        {
            foreach(var file in files.ToList())
            {
                if(file.IsDirectory && file.Children?.Count != 0)
                {
                    return true;
                }
                else if (file.IsDirectory)
                {
                    CheckEmptyFolders(file.Children);
                }
            }
            return false;
        }

        //private bool CheckEmptyFolders()
        //{
        //    //1 prohod
        //    //Если вернет false тогда вернуть коллекцию файлов
        //}
        #endregion
    }
}
