using Avalonia.Data.Converters;
using Avalonia.Media;
using FileControlAvalonia.Core.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Converters
{
    public class ForegroundConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is StatusFile)
            {
                switch (value)
                {
                    case StatusFile.Checked:
                        return Brushes.Black;
                    case StatusFile.PartiallyChecked:
                        return Brushes.Black;
                    case StatusFile.NoAccess:
                        return Brushes.White;
                    case StatusFile.FailedChecked:
                        return Brushes.White;
                    case StatusFile.NotChecked:
                        return Brushes.White;
                    case StatusFile.NotFound:
                        return Brushes.White;
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
