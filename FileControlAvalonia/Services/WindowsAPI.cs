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
        public static string programProcessName = "FileControlAvalonia";
        public static string GetActiveProcessName()
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            GetWindowThreadProcessId(foregroundWindow, out uint processId);

            Process process = Process.GetProcessById((int)processId);
            string processName = process.ProcessName;
            string mainWindowTitle = process.MainWindowTitle;

            return processName;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    }
}