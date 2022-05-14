using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MT5SignalReceiver.ValueConverters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;

            bool invert = parameter != null ? System.Convert.ToBoolean(parameter) : false ;

            return System.Convert.ToBoolean(value) ? (invert ? Visibility.Collapsed : Visibility.Visible) : (invert ? Visibility.Visible : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;

            return ((Visibility)value) == Visibility.Visible ? true : false;

        }
    }
}
