using DynamicData;
using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public class ParallelProcessing
    {
        public static int count = 0;
        public static object _lock = new object();
        public static void ParallelCalculateFactParametrs(ObservableCollection<FileTree> files, int countFiles)
        {
            Calculate(files);

            static void Calculate(ObservableCollection<FileTree> files)
            {
                Task.Run(() =>
                 {
                     foreach (FileTree file in files)
                     {
                         FactParameterizer.SetFactValues(file);
                         lock (_lock)
                         {
                             count++;
                         }
                         if (file.IsDirectory)
                             Calculate(file.Children);
                     }
                 });
            }
            while (true)
            {
                if (count == countFiles)
                {
                    count = 0;
                    break;
                }
            }
        }

    }
}
