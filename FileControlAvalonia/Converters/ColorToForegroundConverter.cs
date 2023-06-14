using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Converters
{
    public class ColorToForegroundConverter : IMultiValueConverter
    {

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values[0] is Brush backgroundBrush && values[1] is Brush defaultForegroundBrush)
            {
                if (backgroundBrush.ToString() == "#f00")
                {
                    return Brushes.White; // Если фон красный, возвращаем белый передний план
                }
            }

            // Возвращаем передний план по умолчанию
            return null;
        }
    }
}
