using Avalonia.FreeDesktop.DBusIme;
using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.FileTreeLogic
{
    public class IdenticalElementChecker
    {
        public static void CheckAndAddMisingElements(ObservableCollection<FileTree> oldFileTrees, IEnumerable<FileTree> newFileTrees)
        {
            foreach (var file in newFileTrees)
            {
                if (!oldFileTrees.Any(x=> x.Path == file.Path))
                {
                    oldFileTrees.Add(file); 
                }
                else if (oldFileTrees.Any(x => x.Path == file.Path) && file.IsDirectory)
                {
                    CheckAndAddMisingElements(oldFileTrees.Where(x=>x.Path == file.Path).FirstOrDefault()!.Children!, file.Children!);
                }
            }
        }
    }
}
