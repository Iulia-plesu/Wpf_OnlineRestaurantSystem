using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Wpf_OnlineRestaurantSystem.Models;

namespace Wpf_OnlineRestaurantSystem.Converters
{
    public class OrderStatusUpdateConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return null;

            if (values[0] is int orderId && values[1] is string newStatus)
            {
                return new OrderStatusUpdateInfo
                {
                    OrderId = orderId,
                    NewStatus = newStatus
                };
            }

            if (int.TryParse(values[0]?.ToString(), out int parsedId) && values[1] is string status)
            {
                return new OrderStatusUpdateInfo
                {
                    OrderId = parsedId,
                    NewStatus = status
                };
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
