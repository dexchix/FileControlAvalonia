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
        //[DllImport("libc")]
        //public static extern int read(int handle, byte[] buf, int n);

        [DllImport("libX11")]
        private static extern IntPtr XOpenDisplay(IntPtr display);

        [DllImport("libX11")]
        private static extern void XCloseDisplay(IntPtr display);

        [DllImport("libX11")]
        private static extern IntPtr XGetInputFocus(IntPtr display, out IntPtr window, out int revertToReturn);

        [DllImport("libXtst")]
        private static extern int XGetWindowProperty(IntPtr display, IntPtr window, IntPtr property, IntPtr offset, IntPtr length,
            int delete, IntPtr req_type, out IntPtr actual_type, out int actual_format, out IntPtr nitems, out IntPtr bytes_after, out IntPtr prop);

        [DllImport("libX11")]
        private static extern IntPtr XFree(IntPtr data);

        public static IntPtr GetActiveWindow()
        {
            IntPtr display = XOpenDisplay(IntPtr.Zero);
            IntPtr activeWindow;
            int revertToReturn;

            XGetInputFocus(display, out activeWindow, out revertToReturn);

            XCloseDisplay(display);

            return activeWindow;
        }

        public static string GetActiveWindowTitle(IntPtr window)
        {
            IntPtr display = XOpenDisplay(IntPtr.Zero);
            IntPtr wmNameProperty = GetAtom(display, "_NET_WM_NAME");
            IntPtr utf8StringType;
            int format;
            IntPtr itemCount, bytesAfter;
            IntPtr propertyName;
            XGetWindowProperty(display, window, wmNameProperty, IntPtr.Zero, new IntPtr(4), 0, IntPtr.Zero, out utf8StringType, out format, out itemCount, out bytesAfter, out propertyName);

            IntPtr wmNamePtr = IntPtr.Zero;
            if (itemCount.ToInt32() > 0)
            {
                XGetWindowProperty(display, window, propertyName, IntPtr.Zero, bytesAfter, 0, utf8StringType, out utf8StringType, out format, out itemCount, out bytesAfter, out wmNamePtr);
            }

            XFree(wmNameProperty);
            XFree(propertyName);

            string wmName = Marshal.PtrToStringUTF8(wmNamePtr);
            XFree(wmNamePtr);

            XCloseDisplay(display);

            return wmName;
        }

        private static IntPtr GetAtom(IntPtr display, string atomName)
        {
            return XInternAtom(display, atomName, false);
        }

        [DllImport("libX11")]
        private static extern IntPtr XInternAtom(IntPtr display, string atom_name, bool only_if_exists);
    }
}
