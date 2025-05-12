using System;
using System.Globalization;
using System.Windows.Data;

namespace Wpf_OnlineRestaurantSystem.Converters
{
    public class QuantityDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return " ";

            string quantity = value.ToString();

            if (string.IsNullOrWhiteSpace(quantity))
                return " ";

            if (int.TryParse(quantity, out int plainNumber))
                return $"Quantity available: {plainNumber}";

            var numericPart = new string(quantity.TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(numericPart, out int numericValue))
                return $"Quantity available: {numericValue} {new string(quantity.Skip(numericPart.Length).ToArray())}";

            return $"Quantity available: {quantity}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}