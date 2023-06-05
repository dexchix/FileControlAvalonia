using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
        #endregion

        #region PROPERTIES
        public string Color { get; set; }
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
        List<string> Colors = new List<string>() { "#f00", "#090", "#FFFF00" };
        #endregion

        public FileTree(string path, bool isDirectory, FileTree? parent = null, bool isRoot = false)
        {
            Color = Colors[new Random().Next(0, 3)];
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

            foreach (var d in Directory.EnumerateDirectories(Path, "*", options))
            {
                result.Add(new FileTree(d, true, this));
            }

            foreach (var f in Directory.EnumerateFiles(Path, "*", options))
            {
                result.Add(new FileTree(f, false, this));
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
