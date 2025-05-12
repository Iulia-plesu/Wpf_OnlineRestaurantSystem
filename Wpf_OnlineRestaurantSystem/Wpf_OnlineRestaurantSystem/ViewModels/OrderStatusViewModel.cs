using System.Collections.ObjectModel;
using Wpf_OnlineRestaurantSystem.Models;

namespace Wpf_OnlineRestaurantSystem.ViewModels
{
    public class OrderStatusViewModel
    {
        public ObservableCollection<UserOrder> Orders { get; set; }

        public OrderStatusViewModel(int userId)
        {
            var ordersFromDb = OrderDAL.GetUserOrders(userId);
            Orders = new ObservableCollection<UserOrder>(ordersFromDb);
        }
    }
}
