using Avalonia.Data.Converters;
using Avalonia.Media;
using FileControlAvalonia.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Converters
{
    public class BackGroundConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is StatusFile)
            {
                switch (value)
                {
                    case StatusFile.Checked:
                        return new SolidColorBrush(Color.Parse("#090"));
                    case StatusFile.PartiallyChecked:
                        return new SolidColorBrush(Color.Parse("#FFFF00"));
                    case StatusFile.NoAccess:
                        return new SolidColorBrush(Color.Parse("#f00"));
                    case StatusFile.FailedChecked:
                        return new SolidColorBrush(Color.Parse("#f00"));
                    case StatusFile.Missing:
                        return new SolidColorBrush(Color.Parse("#f00"));
                    case StatusFile.UnChecked:
                        return new SolidColorBrush(Color.Parse("#f00"));
                }
            }
            return null;

        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
