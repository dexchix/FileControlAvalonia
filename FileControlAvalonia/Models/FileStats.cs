using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileControlAvalonia.Core.Enums;

namespace FileControlAvalonia.Models
{
    public class FileStats
    {
        public int TotalFiles { get; private set; } = 0;
        public int Checked { get; private set; } = 0;
        public int PartialChecked { get; private set; } = 0;
        public int FailedChecked { get; private set; } = 0;
        public int NoAccess { get; private set; } = 0;
        public int NotFound { get; private set; } = 0;
        public int NotChecked { get; private set; } = 0;

        private void CountFileTypes(IEnumerable<FileTree> files)
        {
            foreach (var file in files)
            {
                if (file.IsDirectory)
                {
                    CountFileTypes(file.Children);
                    continue;
                }
                else
                {
                    switch (file.Status)
                    {
                        case StatusFile.Checked:
                            {
                                TotalFiles++;
                                Checked++;
                                break;
                            }

                        case StatusFile.PartiallyChecked:
                            {
                                TotalFiles++;
                                PartialChecked++;
                                break;
                            }

                        case StatusFile.FailedChecked:
                            {
                                TotalFiles++;
                                FailedChecked++;
                                break;
                            }

                        case StatusFile.NoAccess:
                            {
                                TotalFiles++;
                                NoAccess++;
                                break;
                            }

                        case StatusFile.NotFound:
                            {
                                TotalFiles++;
                                NotFound++;
                                break;
                            }

                        case StatusFile.NotChecked:
                            {
                                TotalFiles++;
                                NotChecked++;
                                break;
                            }
                    }
                }
            }
        }
        public FileStats GetFilesStats(IEnumerable<FileTree> filesCollection)
        {
            CountFileTypes(filesCollection);
            return this;
        }
    }
}
