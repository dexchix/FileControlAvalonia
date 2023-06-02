using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System.Diagnostics;

namespace FileControlAvalonia
{
    public class WindowsAPI
    {
        private const int GW_HWNDNEXT = 2;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, IntPtr lpString, int nMaxCount);

        public static string GetActiveWindow()
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            int processId;
            GetWindowThreadProcessId(foregroundWindow, out processId);
            Process process = Process.GetProcessById(processId);
            string windowTitle = GetWindowTitle(foregroundWindow);

            return $"{process.ProcessName} - {windowTitle}";
        }

        private static string GetWindowTitle(IntPtr hWnd)
        {
            const int nChars = 256;
            IntPtr lpString = Marshal.AllocHGlobal(nChars * 2);
            GetWindowText(hWnd, lpString, nChars);
            string windowTitle = Marshal.PtrToStringAuto(lpString);
            Marshal.FreeHGlobal(lpString);
            return windowTitle;
        }

        public static string GetNextWindow()
        {
            IntPtr currentWindow = GetForegroundWindow();
            IntPtr nextWindow = GetWindow(currentWindow, GW_HWNDNEXT);
            string windowTitle = GetWindowTitle(nextWindow);
            return windowTitle;
        }
    }
}