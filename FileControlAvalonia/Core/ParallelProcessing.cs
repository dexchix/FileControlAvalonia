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


        public static void ParallelCalculateFactParametrs1(List<FileTree> files, int countFiles)
        {
            var qqqq = DateTime.Now;
            Task.Run(() =>
            {
                for (int i = 0; i < 21010; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 21010; i < 42020; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 42020; i < 63030; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 63030; i < 84040; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 84040; i < 105050; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 105050; i < 126060; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 126060; i < 147070; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 147070; i < 168080; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 168080; i < 210100; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 210100; i < 231110; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 231110; i < 252120; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 252120; i < 273130; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 273130; i < 294140; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 294140; i < 315150; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            Task.Run(() =>
            {
                for (int i = 315150; i < 336160; i++)
                {
                    FactParameterizer.SetFactValues(files[i]);
                    lock (_lock)
                    {
                        count++;
                    }
                }
            });
            //Task.Run(() =>
            //{
            //    for (int i = 336160; i < 336168; i++)
            //    {
            //        FactParameterizer.SetFactValues(files[i]);
            //        lock (_lock)
            //        {
            //            count++;
            //        }
            //    }
            //});
            while (true)
            {
                if (count == countFiles-200)
                {
                    var sdfsd = DateTime.Now;
                    count = 0;
                    break;
                }
            }
        }
        public static void ParallelCalculateFactParametrs2(List<FileTree> files, int countFiles)
        {

        }
    }
}
