using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Models
{
    public class Watcher: ReactiveObject
    {
        public FileSystemWatcher? _watcher;
        private static string? _rootFolderPath;
        private FileTreeNavigator _fileTreeNavigator;
        public FileTreeNavigator FileTreeNavigator
        {
            get => _fileTreeNavigator;
            set => this.RaiseAndSetIfChanged(ref _fileTreeNavigator, value);
        }
        public Watcher(string rootFolderPath, FileTreeNavigator _fileTreeNavigator)
        {
            _rootFolderPath = rootFolderPath;
            this._fileTreeNavigator = _fileTreeNavigator;
            StartWatch();
        }
        private void StartWatch()
        {
            _watcher = new FileSystemWatcher()
            {
                Path = _rootFolderPath!,
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite |
                               NotifyFilters.FileName |
                               NotifyFilters.DirectoryName |
                               NotifyFilters.Size,
            };
            _watcher.Created += CreatedFile;
            _watcher.Deleted += DeleteFile;
            _watcher.Renamed += RenamedFile;
        }
        /// <summary>
        /// Метод обработчик FSW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreatedFile(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    var parent = FileTreeNavigator.SearchFile(Path.GetDirectoryName(e.FullPath)!, FileTreeNavigator.FileTree);
                    if (parent.Children?.Where(x => x.Path == e.FullPath).FirstOrDefault() != null) return;
                    var addFile = new FileTree(e.FullPath, Directory.Exists(e.FullPath), parent);
                    addFile.IsChecked = addFile.Parent != null && addFile.Parent.IsChecked != false;
                    addFile.HasChildren = addFile.IsDirectory && addFile.Children?.Count != 0 || addFile.Children != null;
                    addFile.Parent!.HasChildren = true;
                    parent.Children!.Add(addFile);
                }
                catch (Exception ex)
                {
                    Program.logger.Error($"Ошибка добавления файла. {ex.Message}");
                }
            });
        }
        /// <summary>
        /// Метод обработчик FSW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFile(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    var file = FileTreeNavigator.SearchFile(e.FullPath, FileTreeNavigator.FileTree);
                    if (file == null) return;
                    file.Parent!.Children?.Remove(file);
                    if (e.FullPath == FileTreeNavigator.FileTree.Path)
                        FileTreeNavigator.GoBackFolder();
                }
                catch (Exception ex)
                {
                    Program.logger.Error($"Ошибка удаления файла. {ex.Message}");
                }
            });
        }
        /// <summary>
        /// Метод обработчик FSW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenamedFile(object sender, RenamedEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    var changedFile = FileTreeNavigator.SearchFile(e.OldFullPath, FileTreeNavigator.FileTree);
                    var duplicat = changedFile.Parent!.Children?.Where(x => x.Path == e.FullPath).FirstOrDefault();
                    changedFile.Parent.Children?.Remove(duplicat!);
                    changedFile.Path = e.FullPath;
                    changedFile.Name = Path.GetFileName(e.FullPath);
                    if (Directory.Exists(changedFile.Path))
                        FileTreeNavigator.ChangePathChildren(changedFile);
                }
                catch (Exception ex)
                {
                    StartWatch();
                    Program.logger.Error($"Ошибка переименования файла. {ex.Message}");
                }
            });
        }
    }
}
