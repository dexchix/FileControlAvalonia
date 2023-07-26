using Avalonia.Controls;
using Avalonia.OpenGL;
using FileControlAvalonia.Core;
using FileControlAvalonia.SettingsApp;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileControlAvalonia.Models
{
    [Serializable]
    public class FileTree : ReactiveObject
    {
        #region FIELDS
        public static int _counter = -1;
        private int _id;
        private int _idParent;
        private bool _loadChildren;
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
        #endregion

        #region PROPERTIES
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isChecked, value);
                if (HasChildren)
                {
                    Task.Run(() =>
                    {
                        foreach (var child in Children!)
                        {
                            child.IsChecked = value;
                        }
                    });
                }
            }
        }
        public int ID
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }
        public int IdParent
        {
            get => _idParent;
            set => this.RaiseAndSetIfChanged(ref _idParent, value);
        }
        public string EHash
        {
            get => _eHash;
            set => this.RaiseAndSetIfChanged(ref _eHash, value);
        }
        public string FHash
        {
            get
            {
                if (File.Exists(Path))
                {
                    return HashSumCalculator.CalculateMD5Hash(Path);
                }
                else
                    return "-";
            }
            set => this.RaiseAndSetIfChanged(ref _fHash, value);
        }
        public string ELastUpdate
        {
            get => _eLastUpdate;
            set => this.RaiseAndSetIfChanged(ref _eLastUpdate, value);
        }
        public string FLastUpdate
        {
            get
            {
                if (IsDirectory)
                {
                    return new DirectoryInfo(Path).LastWriteTime.ToString();
                }
                else
                {
                    return new FileInfo(Path).LastWriteTime.ToString();
                }
            }
            set => this.RaiseAndSetIfChanged(ref _fLastUpdate, value);
        }
        public string EVersion
        {
            get
            {
                return _eVersion;
            }
            set => this.RaiseAndSetIfChanged(ref _eVersion, value);
        }
        public string FVersion
        {
            get
            {
                if (File.Exists(Path))
                {
                    var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(Path);
                    return versionInfo.FileVersion == null || versionInfo.FileVersion == "" ? "-" : versionInfo.FileVersion!;
                }
                else return "-";
            }
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

        public bool IsDirectory { get; }
        public FileTree? Parent { get; set; }
        public ObservableCollection<FileTree>? Children
        {
            get => _children ??= LoadChildren();
            set => this.RaiseAndSetIfChanged(ref _children, value);
            //get
            //{
            //    if (_loadChildren == true)
            //        return _children ??= LoadChildren();
            //    else
            //        return new ObservableCollection<FileTree>();
            //}
            //set => this.RaiseAndSetIfChanged(ref _children, value);
        }
        #endregion

        public FileTree(string path, bool isDirectory, FileTree? parent = null, bool isRoot = false, bool loadChildren = true)
        {
            _counter++;
            _id = _counter;
            if (parent != null) _idParent = parent.ID;
            _path = path;
            _name = isRoot ? path : System.IO.Path.GetFileName(Path);
            _isExpanded = isRoot;
            _loadChildren = loadChildren;
            IsDirectory = isDirectory;
            HasChildren = isDirectory;
            _isChecked = false;
            Parent = parent;
            _children = LoadChildren();
        }

        private ObservableCollection<FileTree>? LoadChildren()
        {
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
                result.Add(new FileTree(directory, true, this));
            }

            foreach (var file in Directory.EnumerateFiles(Path, "*", options))
            {
                if (extensions == null || extensions.Count == 0 || extensions[0] == "")
                {
                    var children = new FileTree(file, false, this);
                    result.Add(children);
                }
                else if (extensions.Contains(System.IO.Path.GetExtension(file)))
                {
                    var children = new FileTree(file, false, this);
                    result.Add(children);
                }
            }

            if (result.Count == 0)
                HasChildren = false;

            return result;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public override string ToString()
        {
            return Path;
        }
    }
}
