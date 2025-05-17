using System;
using System.Globalization;
using System.Windows.Data;
using Wpf_OnlineRestaurantSystem.Models;
namespace Wpf_OnlineRestaurantSystem.Converters
{
    public class CancelOrderInfoConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2) return null;

            if (values[0] is int orderId && values[1] is UserWithOrders user)
            {
                var userOrder = user.Orders?.FirstOrDefault(o => o.OrderId == orderId);
                if (userOrder != null)
                {
                    return new CancelOrderInfo
                    {
                        OrderId = orderId,
                        UserId = userOrder.UserId
                    };
                }
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}