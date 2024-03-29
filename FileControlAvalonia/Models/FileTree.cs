﻿using Avalonia.Input.TextInput;
using FileControlAvalonia.Core.Enums;
using FileControlAvalonia.SettingsApp;
using FileControlAvalonia.ViewModels;
using ReactiveUI;
using SQLite;
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
        private string? _eHash;
        private string? _fHash;
        private string? _eLastUpdate;
        private string? _fLastUpdate;
        private string? _eVersion;
        private string? _fVersion;
        private string? _path;
        private string? _name;
        private ObservableCollection<FileTree>? _children;
        private bool _hasChildren = true;
        private bool _isExpanded;
        private bool _isChecked;
        private StatusFile _status = StatusFile.Checked;
        private bool _loadChildren;
        private static object _lock = new object();
        public static List<Task> _startedTask = new List<Task>();
        public static event Action SelectedFolder = FileExplorerWindowViewModel.ChangeStateProgressBarMain;
        public static bool TaskSelectedChildrenIsStarted = false;
        public string _parentPath;
        #endregion

        #region PROPERTIES

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [Ignore]
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isChecked, value);

                if (HasChildren && TaskSelectedChildrenIsStarted == false)
                {
                    TaskSelectedChildrenIsStarted = true;

                    _startedTask.Add(Task.Run(() =>
                    {
                        SelectedChildren(value, this);
                    }));


                    SelectedFolder.Invoke();
                }
            }
        }

        public string ParentPath
        {
            get => _parentPath;

            set => _parentPath = value;

        }

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
        [Column("Status")]
        public StatusFile Status
        {
            get => _status;
            set
            {
                if(Path == @"C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS")
                {

                }
                this.RaiseAndSetIfChanged(ref _status, value);
            }
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
        [Ignore]
        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }
        [Ignore]
        public bool IsOpened { get; set; }

        public bool IsDirectory { get; set; }
        [Ignore]
        public FileTree? Parent { get; set; }
        [Ignore]
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



            if (Parent != null)
            {
                if (Parent.Path == SettingsManager.RootPath)
                    ParentPath = null;
                else
                    ParentPath = Parent.Path;
            }
            else
                ParentPath = null;
        }
        public FileTree()
        {

        }

        private void SelectedChildren(bool value, FileTree fileTree)
        {
            foreach (var child in fileTree.Children!.ToList())
            {
                lock (_lock)
                {
                    child.IsChecked = value;
                }
                if (child.IsDirectory) SelectedChildren(value, child);
            }
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
            return $"{Path} | ID = {ID}";
        }
    }
}
