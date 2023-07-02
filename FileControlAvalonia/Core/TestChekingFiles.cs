using FileControlAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public class TestChekingFiles
    {
        public static void TEST(FileTree files)
        {
            foreach (FileTree file in files.Children!.ToList())
            {
                file.Status = TEST2();
                if (file.IsDirectory)
                {
                    TEST(file);
                }
            }
        }
        private static StatusFile TEST2()
        {
            var rnd = new Random().Next(0, 6);
            switch (rnd)
            {
                case 0:
                    return StatusFile.Checked;
                case 1:
                    return StatusFile.PartiallyChecked;
                case 2:
                    return StatusFile.FailedChecked;
                case 3:
                    return StatusFile.UnChecked;
                case 4:
                    return StatusFile.NoAccess;
                case 5:
                    return StatusFile.Missing;
            }
            return StatusFile.UnChecked;
        }
    }
}
