using FileControlAvalonia.Core;
using FileControlAvalonia.Models;
using NLog;
using ReactiveUI;
using Splat;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileControlAvalonia.FileTreeLogic
{
    public class FileTreeNavigator : ReactiveObject
    {
        #region FIELDS
        //public static readonly string pathRootFolder = "C:\\1\\5";
        //public static readonly string pathRootFolder = "/home/orpo/Desktop/1/2";
        public static FileTree? fileTree;
        public static Watcher? watcher;
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
            if (Directory.Exists(SettingsManager.rootPath))
            {
                fileTree = new FileTree(SettingsManager.rootPath, true);
                watcher = new Watcher(SettingsManager.rootPath, this);
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
        public static FileTree SearchFile(string searchedFilePath, FileTree fileTree)
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
                    if (!Directory.Exists(SettingsManager.rootPath))
                    {
                        CheckExistRootPath();
                        return;
                    }
                }
            });
        }
        #endregion
    }
}
