using System;
using System.Globalization;
using System.Windows.Data;

namespace Wpf_OnlineRestaurantSystem.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool flag && flag) ? 1.0 : 0.4; // 40% opac dacă nu e conectat
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
