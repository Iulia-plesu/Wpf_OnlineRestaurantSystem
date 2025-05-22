using System;
using System.Globalization;
using System.Windows.Data;
using  Wpf_OnlineRestaurantSystem.ViewModels;

namespace Wpf_OnlineRestaurantSystem.Converters
{
    public class OrderIdDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int orderId)
            {
                return AdminViewModel.OrderIdEncoder.EncodeOrderId(orderId);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}