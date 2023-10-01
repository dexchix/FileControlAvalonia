using FileControlAvalonia.Core;
using FileControlAvalonia.SettingsApp;
using FileControlAvalonia.ViewModels;
using FileControlAvalonia.Views;
using Microsoft.CodeAnalysis.FlowAnalysis;
using OpcXml.Da;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileControlAvalonia.Models
{
    [Serializable]
    public class FileTree : ReactiveObject
    {
        #region FIELDS
        public static int _counter { get; set; } = -1;
        private string _eHash;
        private string _fHash;
        private string _eLastUpdate;
        private string _fLastUpdate;
        private string _eVersion;
        private string _fVersion;
        private string _path;
        private string _name;
        private ObservableCollection<FileTree>? _children;
        private bool _hasChildren = true;
        private bool _isExpanded;
        private bool _isChecked;
        private StatusFile _status = StatusFile.Checked;
        private bool _loadChildren;
        public static int CountSelectedFiles { get; set; } = 0;
        private static object _lock = new object();
        public static List<Task> _startedTask = new List<Task>();
        public static bool progressBarIsActive = false;
        public static event Action SelectedFolder = FileExplorerWindowViewModel.ChangeStateProgressBarMain;
        #endregion

        #region PROPERTIES
        public bool IsChecked
        {
            get => _isChecked;
            //set
            //{
            //    this.RaiseAndSetIfChanged(ref _isChecked, value);

            //    if (HasChildren)
            //    {
            //        FileExplorerWindowViewModel._currentWM.ProgressBarIsVisible = true;
            //        FileExplorerWindowViewModel._currentWM.ProgressBarLoopScrol = true;
            //        FileExplorerWindowViewModel._currentWM.EnabledButtons = false;
            //        foreach (var child in Children!.ToList())
            //        {
            //            //Task.Run(() =>
            //            //{
            //            //    child.IsChecked = value;
            //            //    lock (_lock)
            //            //    {
            //            //        if (value == true) CountSelectedFiles++;
            //            //        else CountSelectedFiles--;
            //            //    }
            //            //});
            //            _startedTask.Add(Task.Run(() =>
            //            {
            //                child.IsChecked = value;
            //                lock (_lock)
            //                {
            //                    if (value == true) CountSelectedFiles++;
            //                    else CountSelectedFiles--;
            //                }
            //            }));
            //        }
            //        CheckSelectedFiles();
            //    }
            //}
            set => SetIsCheckedRecursivelyAsync(value);
        }
        public async Task SetIsCheckedRecursivelyAsync(bool value)
        {
            //await Task.WhenAll(FileTree._startedTask.ToList());
            lock (_lock)
            {
                this.RaiseAndSetIfChanged(ref _isChecked, value);
                if (value)
                    CountSelectedFiles++;
                else
                    CountSelectedFiles--;
            }

            if (HasChildren)
            {
                if (progressBarIsActive == false)
                    Task.Run(() => SelectedFolder.Invoke());

                List<Task> childTasks = new List<Task>();
                foreach (var child in Children!.ToList())
                {
                    lock (_lock)
                    {
                        Task childTasl = (Task.Run(async () =>
                        {

                            if (value)
                                CountSelectedFiles++;
                            else
                                CountSelectedFiles--;
                            child.SetIsCheckedRecursivelyAsync(value); // Ждем завершения асинхронной задачи
                        }));
                        //childTasks.Add(childTasl);
                        _startedTask.Add(childTasl);
                    }
                }
                //await Task.WhenAll(childTasks); // Дождитесь завершения всех асинхронных задач

                //await Task.WhenAll(_startedTask.ToList());

                //ChangeStateProgressBar(false);
            }
        }
        //public async Task SetIsCheckedRecursivelyAsync(bool value)
        //{
        //    this.RaiseAndSetIfChanged(ref _isChecked, value);
        //    if (value)
        //        CountSelectedFiles++;
        //    else
        //        CountSelectedFiles--;

        //    if (HasChildren)
        //    {
        //        if (!progressBarIsActive)
        //            Task.Run(() => SelectedFolder.Invoke());

        //        var childTasks = new List<Task>();
        //        foreach (var child in Children!.ToList())
        //        {
        //            childTasks.Add(SetChildIsCheckedRecursivelyAsync(child, value));
        //        }

        //        await Task.WhenAll(childTasks);
        //    }
        //}

        //private async Task SetChildIsCheckedRecursivelyAsync(FileTree child, bool value)
        //{
        //    // Обновление child.IsChecked и CountSelectedFiles без блокировки, если это безопасно.

        //    if (child.HasChildren)
        //    {
        //        var grandChildTasks = new List<Task>();
        //        foreach (var grandChild in child.Children!.ToList())
        //        {
        //            grandChildTasks.Add(SetChildIsCheckedRecursivelyAsync(grandChild, value));
        //        }

        //        await Task.WhenAll(grandChildTasks);
        //    }
        //}

        public string EHash
        {
            get => _eHash;
            set => this.RaiseAndSetIfChanged(ref _eHash, value);
        }
        public string FHash
        {
            get => _fHash;
            set => this.RaiseAndSetIfChanged(ref _fHash, value);
        }
        public string ELastUpdate
        {
            get => _eLastUpdate;
            set => this.RaiseAndSetIfChanged(ref _eLastUpdate, value);
        }
        public string FLastUpdate
        {
            get => _fLastUpdate;
            set => this.RaiseAndSetIfChanged(ref _fLastUpdate, value);
        }
        public string EVersion
        {
            get => _eVersion;
            set => this.RaiseAndSetIfChanged(ref _eVersion, value);
        }
        public string FVersion
        {
            get => _fVersion;
            set => this.RaiseAndSetIfChanged(ref _fVersion, value);
        }
        public StatusFile Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }
        public string Path
        {
            get => _path;
            set => this.RaiseAndSetIfChanged(ref _path, value);
        }
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public bool HasChildren
        {
            get => _hasChildren;
            set => this.RaiseAndSetIfChanged(ref _hasChildren, value);
        }
        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }
        public bool IsOpened { get; set; }

        public bool IsDirectory { get; }
        public FileTree? Parent { get; set; }
        public ObservableCollection<FileTree>? Children
        {
            get
            {
                if (_loadChildren)
                    return _children ??= LoadChildren();
                else
                    return _children ??= new ObservableCollection<FileTree>();
            }
            set => this.RaiseAndSetIfChanged(ref _children, value);
        }
        #endregion

        public FileTree(string path, bool isDirectory, bool loadChildren = false, FileTree? parent = null, bool isRoot = false)
        {
            _path = path;
            _name = isRoot ? path : System.IO.Path.GetFileName(Path);
            _isExpanded = isRoot;
            IsDirectory = isDirectory;
            HasChildren = isDirectory;
            _isChecked = false;
            Parent = parent;
            _loadChildren = loadChildren;
        }

        private ObservableCollection<FileTree>? LoadChildren()
        {
            try
            {
                IsOpened = true;
                var extensions = SettingsManager.ModifyExtensions;

                if (!IsDirectory)
                {
                    return null;
                }
                var options = new EnumerationOptions()
                {
                    IgnoreInaccessible = true,
                    AttributesToSkip = default
                };
                var result = new ObservableCollection<FileTree>();

                foreach (var directory in Directory.EnumerateDirectories(Path, "*", options))
                {
                    result.Add(new FileTree(directory, true, _loadChildren, this));
                }

                foreach (var file in Directory.EnumerateFiles(Path, "*", options))
                {
                    if (extensions == null || extensions.Count == 0 || extensions[0] == string.Empty)
                    {
                        var children = new FileTree(file, false, _loadChildren, this);
                        result.Add(children);
                    }
                    else if (extensions.Contains(System.IO.Path.GetExtension(file)))
                    {
                        var children = new FileTree(file, false, _loadChildren, this);
                        result.Add(children);
                    }
                }

                if (result.Count == 0)
                    HasChildren = false;

                return result;
            }
            catch
            {
                return new ObservableCollection<FileTree>();
            }

        }
        public override string ToString()
        {
            return Path;
        }
    }
}
