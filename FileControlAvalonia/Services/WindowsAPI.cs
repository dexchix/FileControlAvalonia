using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace CRCApplication
{
    class WINAPI
    {
        //const int SC_CLOSE = 0xF010;
        //const int MF_BYCOMMAND = 0;
        const int WM_NCLBUTTONDOWN = 0x00A1;
        const int WM_NCHITTEST = 0x0084;
        const int HTCAPTION = 2;
        public const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        public const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("User32.dll")]
        static extern int SendMessage(IntPtr hWnd,
        int Msg, IntPtr wParam, IntPtr lParam);

        //[DllImport("User32.dll")]
        //static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        //[DllImport("User32.dll")]
        //static extern bool RemoveMenu(IntPtr hMenu, int uPosition, int uFlags);

        internal static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_NCLBUTTONDOWN)
            {
                int result = SendMessage(hwnd, WM_NCHITTEST,
                IntPtr.Zero, lParam);
                if (result == HTCAPTION)
                    handled = true;
            }

            return IntPtr.Zero;
        }

        public static void CloseDialogWindows()
        {
            // Enumerate windows to find dialogs
            EnumThreadWndProc callback = new EnumThreadWndProc(checkWindow);
            EnumThreadWindows(GetCurrentThreadId(), callback, IntPtr.Zero);
            GC.KeepAlive(callback);
        }

        private static bool checkWindow(IntPtr hWnd, IntPtr lp)
        {
            // Checks if <hWnd> is a Windows dialog
            StringBuilder sb = new StringBuilder(260);
            GetClassName(hWnd, sb, sb.Capacity);
            if (sb.ToString() == "#32770")
            {
                // Close it by sending WM_CLOSE to the window
                SendMessage(hWnd, 0x0010, IntPtr.Zero, IntPtr.Zero);
            }
            return true;
        }

        // P/Invoke declarations
        private delegate bool EnumThreadWndProc(IntPtr hWnd, IntPtr lp);
        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(int tid, EnumThreadWndProc callback, IntPtr lp);
        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();
        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder buffer, int buflen);
    }
    //public static class DialogCloser
    //{
    //    public static readonly DependencyProperty DialogResultProperty =
    //        DependencyProperty.RegisterAttached(
    //            "DialogResult",
    //            typeof(bool?),
    //            typeof(DialogCloser),
    //            new PropertyMetadata(DialogResultChanged));

    //    private static void DialogResultChanged(
    //        DependencyObject d,
    //        DependencyPropertyChangedEventArgs e)
    //    {
    //        var window = d as Window;
    //        if (window != null)
    //        {
    //            window.DialogResult = e.NewValue as bool?;
    //            if (window.DialogResult != null)
    //                window.Close();
    //        }
    //    }

    //    public static void SetDialogResult(Window target, bool? value)
    //    {
    //        target.SetValue(DialogResultProperty, value);
    //    }
    //}

    //public class UserLevelToBooleanConverter : MarkupExtension, IValueConverter
    //{
    //    public override object ProvideValue(IServiceProvider serviceProvider)
    //    {
    //        return this;
    //    }

    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (value == null)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            if ((UserLevels)value == UserLevels.Admin | (UserLevels)value == UserLevels.Kurator)
    //            {
    //                return true;
    //            }
    //            return false;
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    //public class UserLevelToVisibleConverter : MarkupExtension, IValueConverter
    //{
    //    public override object ProvideValue(IServiceProvider serviceProvider)
    //    {
    //        return this;
    //    }

    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (value == null)
    //        {
    //            return null;
    //        }
    //        else
    //        {
    //            List<int> tList = new List<int>();
    //            for (int i = 0; i < parameter.ToString().Length; i++)
    //            {
    //                tList.Add(System.Convert.ToInt32(parameter.ToString()[i].ToString()));
    //            }
    //            if (tList.Contains((int)value))
    //            {
    //                return System.Windows.Visibility.Visible;
    //            }
    //            return System.Windows.Visibility.Collapsed;
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    //public class BoolToVisibleConverter : MarkupExtension, IValueConverter
    //{
    //    public override object ProvideValue(IServiceProvider serviceProvider)
    //    {
    //        return this;
    //    }

        //public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    if (value == null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        if (parameter.ToString() == "True")
        //        {
        //            //return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            //return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        //        }
        //    }
        //}

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class BackBoolConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                return !(bool)value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HelperMethods
    {
        /// <summary>
        /// Получить описание элемента перечисления
        /// </summary>
        /// <param name="enumElement"></param>
        /// <returns></returns>
        public static string GetDescription(Enum enumElement)
        {
            Type type = enumElement.GetType();

            MemberInfo[] memInfo = type.GetMember(enumElement.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return enumElement.ToString();
        }
        /// <summary>
        /// Прочесть аргументы командной строки
        /// </summary>        
        internal static void ReadArguments(ICollection<string> commandLine)
        {
            foreach (var arg in commandLine)
            {
                if (arg.ToUpper().Contains("MONITOR"))
                {
                    int tempMon = Convert.ToInt32(arg.Remove(0, arg.IndexOf("_") + 1));
                    //Models.MainModel.Monitor = tempMon >= System.Windows.Forms.Screen.AllScreens.Length ? 0 : tempMon;
                }
            }
        }
    }
}