using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Converters
{
    public class ArrowConverter : IMultiValueConverter
    {
        private readonly Bitmap _arrayRight;
        private readonly Bitmap _arrayDown;
        private readonly Bitmap _emptyImage;

        public ArrowConverter(Bitmap arrayRight, Bitmap arrayDown, Bitmap emptyImage)
        {
            _arrayDown = arrayRight;
            _arrayRight = arrayDown;
            _emptyImage = emptyImage;
        }

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count == 3 &&
            values[0] is bool isDirectory &&
            values[1] is bool isExpanded &&
            values[2] is bool hasChildren)
            {
                if (!hasChildren && isDirectory)
                {
                    return _emptyImage;
                }
                if (!hasChildren)
                {
                    return null;
                }
                if (isExpanded)
                {
                    return _arrayRight;
                }
                else
                    return _arrayDown;
            }

            return null;
        }
    }
}
