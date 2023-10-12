using FileControlAvalonia.FileTreeLogic;
using FileControlAvalonia.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace FileControlAvalonia.DataBase
{
    public class Converter
    {
        private object _lock = new object();
        private int _count = 0;
        private int fileCounter = 0;

        public ObservableCollection<FileTree> ConvertListToHierarchicalCollection(List<FileTree> files)
        {
            var etalon = new ObservableCollection<FileTree>();
            var filesDictionary = new Dictionary<string, FileTree>();

            var rootsDictionary = new List<FileTree>();

            if (files.Count <= 1000)
            {
                while (fileCounter < files.Count)
                {
                    if (files[fileCounter].ParentPath == null || files[fileCounter].ParentPath == string.Empty)
                    {
                        etalon.Add(files[fileCounter]);
                        fileCounter++;
                    }
                    else
                    {
                        var parent = FileTreeNavigator.SeachFileInFilesCollection(Path.GetDirectoryName(files[fileCounter].Path)!, etalon);
                        parent.Children!.Add(files[fileCounter]);
                        fileCounter++;
                    }
                }
                return etalon;
            }
            else
            {
                int start = 0;
                int limit = 1;
                var section = files.Count / 8;
                int residue = files.Count - section * 8;
                for (int i = 0; i < 8; i++)
                {
                    int localStart = start * section;
                    int localLimit = limit * section;
                    start++;
                    limit++;
                    Task.Run(() =>
                    {
                        for (int i = localStart; i < localLimit; i++)
                        {
                            lock (_lock)
                            {
                                filesDictionary.Add(files[i].Path, files[i]);
                                if (files[i].ParentPath == null || files[i].ParentPath == string.Empty)
                                    rootsDictionary.Add(files[i]);
                                _count++;
                            }
                        }
                    });
                }
                for (int i = files.Count - residue; i < files.Count; i++)
                {
                    lock (_lock)
                    {
                        filesDictionary.Add(files[i].Path, files[i]);
                        if (files[i].ParentPath == null || files[i].ParentPath == string.Empty)
                            rootsDictionary.Add(files[i]);
                        _count++;
                    }
                }

                while (true)
                {
                    if (_count == files.Count)
                    {
                        _count = 0;
                        start = 0;
                        limit = 1;
                        break;
                    }
                }
                //Присваивание Parent и Children
                for (int i = 0; i < 8; i++)
                {
                    int localStart = start * section;
                    int localLimit = limit * section;
                    start++;
                    limit++;
                    Task.Run(() =>
                    {
                        try
                        {
                            for (int i = localStart; i < localLimit; i++)
                            {
                                lock (_lock)
                                {
                                    var file = filesDictionary[files[i].Path];
                                    if (files[i].ParentPath != null && files[i].ParentPath != string.Empty)
                                    {
                                        var fileParent = filesDictionary[files[i].ParentPath];
                                        if (fileParent != null)
                                        {
                                            file.Parent = fileParent;
                                            fileParent.Children.Add(file);
                                        }
                                    }
                                    _count++;
                                }
                            }

                        }
                        catch
                        {

                        }
                       
                    });
                }
                for (int i = files.Count - residue; i < files.Count; i++)
                {
                    try
                    {
                        lock (_lock)
                        {
                            var file = filesDictionary[files[i].Path];
                            if (files[i].ParentPath != null && files[i].ParentPath != string.Empty)
                            {
                                var fileParent = filesDictionary[files[i].ParentPath];
                                if (fileParent != null)
                                {
                                    file.Parent = fileParent;
                                    fileParent.Children!.Add(file);
                                }
                            }
                            _count++;
                        }
                    }
                    catch
                    {

                    }
                    
                }
                while (true)
                {
                    if (_count == files.Count)
                    {
                        _count = 0;
                        break;
                    }
                }

                foreach (var file in rootsDictionary)
                {
                    etalon.Add(file);
                }
                return etalon;
            }

        }
    }
}
