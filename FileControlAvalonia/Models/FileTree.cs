using FileControlAvalonia.Core;
using FileControlAvalonia.Services;
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
    public class FileTree : ReactiveObject, ICloneable
    {
        #region FIELDS
        private string _path;
        private string _name;
        private ObservableCollection<FileTree>? _children;
        private bool _hasChildren = true;
        private bool _isExpanded;
        private bool _isChecked;
        private StatusFile _status;
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
        }
        #endregion

        public FileTree(string path, bool isDirectory, FileTree? parent = null, bool isRoot = false)
        {
            _path = path;
            _name = isRoot ? path : System.IO.Path.GetFileName(Path);
            _isExpanded = isRoot;
            IsDirectory = isDirectory;
            HasChildren = isDirectory;
            _isChecked = false;
            Parent = parent;
        }

        private ObservableCollection<FileTree>? LoadChildren()
        {
            var extensions = SettingsManager.modifyExtensions;

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
                    result.Add(new FileTree(file, false, this));
                else if (extensions.Contains(System.IO.Path.GetExtension(file)))
                    result.Add(new FileTree(file, false, this));
            }

            if (result.Count == 0)
                HasChildren = false;

            return result;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
