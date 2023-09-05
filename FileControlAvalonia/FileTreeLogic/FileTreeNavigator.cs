using FileControlAvalonia.Models;
using FileControlAvalonia.SettingsApp;
using NLog;
using ReactiveUI;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileControlAvalonia.FileTreeLogic
{
    public class FileTreeNavigator : ReactiveObject
    {
        #region FIELDS
        //public static readonly string pathRootFolder = "C:\\1\\5";
        //public static readonly string pathRootFolder = "/home/orpo/Desktop/1/2";
        public FileTree? fileTree;
        public Watcher? watcher;
        public CancellationTokenSource cts = new CancellationTokenSource();
        new public event PropertyChangedEventHandler? PropertyChanged;
        #endregion 

        public FileTree FileTree
        {
            get => fileTree!;
            set
            {
                fileTree = value;
                OnPropertyChanged(nameof(FileTree));
            }
        }
        public FileTreeNavigator()
        {
            if (Directory.Exists(SettingsManager.RootPath))
            {
                fileTree = new FileTree(SettingsManager.RootPath, true, true);
                Task.Run(async () =>
                {
                    //await Task.Delay(5000);
                    watcher = new Watcher(SettingsManager.RootPath, this);
                });
                //CheckChangeRootPath();
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
            if (fileTree != null && fileTree.Parent != null)
            {
                FileTree = FileTree.Parent!;
            }
        }
        /// <summary>
        /// Поиск файла в дереве
        /// </summary>
        /// <param name="searchedFilePath"></param>
        /// <returns>Экземпляр типа FileTree (Файл)</returns>
        public static FileTree SearchFileInFileTree(string searchedFilePath, FileTree fileTree)
        {
            if (fileTree.Path == searchedFilePath)
                return fileTree;
            else if (searchedFilePath.StartsWith(fileTree.Path))
                return SearchChildren(searchedFilePath, fileTree)!;
            else if (fileTree.Path.StartsWith(searchedFilePath))
                return SearchTreeParent(searchedFilePath, fileTree);
            else
                return SearchChildren(searchedFilePath, SearchTreeParent(searchedFilePath, fileTree))!;
        }
        /// <summary>
        /// Поиск файлов в коллекции файлов
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static FileTree SeachFileInFilesCollection(string searchedFilePath, ObservableCollection<FileTree> files)
        {
            var rootParant = files.Where(x => searchedFilePath.StartsWith(x.Path))
                                         .OrderByDescending(x => x.Path.Length)
                                         .FirstOrDefault()!;
            var parent = SearchFileInFileTree(searchedFilePath, rootParant);
            return parent;
        }
        /// <summary>
        /// Поиск родительского элемента в дереве
        /// </summary>
        /// <param name="searchedFilePath"></param>
        /// <param name="openedFolder"></param>
        /// <returns>Элемент типа FileTree (Файл)</returns>
        public static FileTree SearchTreeParent(string searchedFilePath, FileTree openedFolder)
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
        public static FileTree? SearchChildren(string searchedFilePath, FileTree rootFolder)
        {
            //var maxMatchFile = rootFolder.Children!.Where(x => searchedFilePath.StartsWith(x.Path))
            //                                      .FirstOrDefault()!;
            var maxMatchFile = rootFolder.Children!
                                         .Where(x => searchedFilePath.StartsWith(x.Path))
                                         .OrderByDescending(x => x.Path.Length)
                                         .FirstOrDefault()!;
            try
            {
                return maxMatchFile.Path == searchedFilePath
                    ? maxMatchFile
                    : SearchChildren(searchedFilePath, maxMatchFile);
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error($"Не удалось найти файл в дереве {ex.Message}");
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
        public void ClearFileExplorerForm()
        {
            try
            {
                FileTree.Path = "Исходный путь отсутствует!";
                FileTree.Children!.Clear();
                FileTree.Parent = null;
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error($"Ошибка отчиски формы. {ex}");
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
                    if (!Directory.Exists(SettingsManager.RootPath))
                    {
                        ClearFileExplorerForm();
                        return;
                    }
                    Task.Delay(1000);
                }
            });
        }


        public static void DisposeFileTree(FileTree fileTree)
        {

        }
        #endregion


        ~FileTreeNavigator()
        {
            if (watcher != null)
                watcher.StopWatch();
        }
    }
}
