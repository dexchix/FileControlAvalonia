using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Services
{
    public class LinuxAPI
    {
        [DllImport("libc")]
        public static extern int read(int handle, byte[] buf, int n);

        

    }
}
