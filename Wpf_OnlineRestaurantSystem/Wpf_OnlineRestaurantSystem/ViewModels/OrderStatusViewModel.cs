using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Wpf_OnlineRestaurantSystem.Models;
using Microsoft.Data.SqlClient;

namespace Wpf_OnlineRestaurantSystem.ViewModels
{
    public class OrderStatusViewModel
    {
        private int _userId;
        public ObservableCollection<UserOrder> Orders { get; set; }
        public ICommand CancelOrderCommand { get; }
        public ICommand RefreshCommand { get; }

        public OrderStatusViewModel(int userId)
        {
            _userId = userId;
            LoadOrders();

            CancelOrderCommand = new RelayCommand(param => CancelOrder((int)param));
            RefreshCommand = new RelayCommand(RefreshOrders);
        }

        private void LoadOrders()
        {
            var ordersFromDb = OrderDAL.GetUserOrders(_userId);
            Orders = new ObservableCollection<UserOrder>(ordersFromDb);
        }

        public void RefreshOrders(object ob)
        {
            LoadOrders(); // Reload orders from database
            MessageBox.Show("Orders refreshed successfully!", "Refresh",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CancelOrder(int orderId)
        {
            try
            {
                OrderDAL.CancelOrder(orderId);
                var order = Orders.FirstOrDefault(o => o.OrderId == orderId);
                if (order != null)
                    order.Status = "Canceled";
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"SQL Error {sqlEx.Number}: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cancelling order: " + ex.Message);
            }
        }
    }
}