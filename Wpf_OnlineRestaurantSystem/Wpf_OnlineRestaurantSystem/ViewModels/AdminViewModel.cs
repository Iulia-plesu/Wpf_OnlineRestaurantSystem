using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Wpf_OnlineRestaurantSystem.Models;
using Wpf_OnlineRestaurantSystem.Views;

namespace Wpf_OnlineRestaurantSystem.ViewModels
{
    public class AdminViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<UserWithOrders> _usersWithOrders;
        public ObservableCollection<UserWithOrders> UsersWithOrders
        {
            get => _usersWithOrders;
            set
            {
                _usersWithOrders = value;
                OnPropertyChanged();
            }
        }

        private int _registeredCount;
        public int RegisteredCount
        {
            get => _registeredCount;
            set { _registeredCount = value; OnPropertyChanged(); }
        }

        private int _inPreparationCount;
        public int InPreparationCount
        {
            get => _inPreparationCount;
            set { _inPreparationCount = value; OnPropertyChanged(); }
        }

        private int _outForDeliveryCount;
        public int OutForDeliveryCount
        {
            get => _outForDeliveryCount;
            set { _outForDeliveryCount = value; OnPropertyChanged(); }
        }

        private int _deliveredCount;
        public int DeliveredCount
        {
            get => _deliveredCount;
            set { _deliveredCount = value; OnPropertyChanged(); }
        }

        private int _canceledCount;
        public int CanceledCount
        {
            get => _canceledCount;
            set { _canceledCount = value; OnPropertyChanged(); }
        }

        public ICommand UpdateOrderStatusCommand { get; }
        public ICommand OpenInventoryManagementCommand { get; }
        public ICommand CancelOrderCommand { get; }

        public AdminViewModel()
        {
            CancelOrderCommand = new RelayCommand(CancelOrder);
            UpdateOrderStatusCommand = new RelayCommand(UpdateOrderStatus);
            OpenInventoryManagementCommand = new RelayCommand(OpenInventoryManagement);

            LoadUsersAndOrders();
        }

        private void LoadUsersAndOrders()
        {
            var result = new ObservableCollection<UserWithOrders>();

            var users = UserDAL.GetNormalUsers();
            foreach (var user in users)
            {
                var orders = OrderDAL.GetUserOrders(user.Id);
                result.Add(new UserWithOrders
                {
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    Orders = orders
                });
            }

            UsersWithOrders = result;
            UpdateOrderStatusCounts();
        }

        private void UpdateOrderStatusCounts()
        {
            if (UsersWithOrders == null) return;

            var allOrders = UsersWithOrders.SelectMany(u => u.Orders).ToList();

            RegisteredCount = allOrders.Count(o => o.Status == "Registered");
            InPreparationCount = allOrders.Count(o => o.Status == "In Preparation");
            OutForDeliveryCount = allOrders.Count(o => o.Status == "Out for Delivery");
            DeliveredCount = allOrders.Count(o => o.Status == "Delivered");
            CanceledCount = allOrders.Count(o => o.Status == "Canceled");
        }

        private void UpdateOrderStatus(object parameter)
        {
            if (parameter is OrderStatusUpdateInfo info)
            {
                OrderDAL.UpdateOrderStatus(info.OrderId, info.NewStatus);

                var user = UsersWithOrders.FirstOrDefault(u => u.Orders.Any(o => o.OrderId == info.OrderId));
                if (user != null)
                {
                    user.Orders = OrderDAL.GetUserOrders(user.Orders.First().UserId);
                    OnPropertyChanged(nameof(UsersWithOrders));
                }
            }
        }

        private void CancelOrder(object parameter)
        {
            if (parameter is CancelOrderInfo info)
            {
                OrderDAL.CancelOrder(info.OrderId);
            }
        }

        private void CancelOrder(CancelOrderInfo info)
        {
            if (info == null) return;

            OrderDAL.CancelOrder(info.OrderId);

            var user = UsersWithOrders.FirstOrDefault(u => u.Orders.Any(o => o.OrderId == info.OrderId));
            if (user != null)
            {
                user.Orders = OrderDAL.GetUserOrders(info.UserId);
                OnPropertyChanged(nameof(UsersWithOrders));
            }
        }

        private void OpenInventoryManagement(object parameter)
        {
            var window = new InventoryManagementWindow();
            window.DataContext = new InventoryManagementViewModel();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public static class OrderIdEncoder
        {
            private static readonly string Prefix = "CR-";
            private static readonly string Salt = "ChâteauRosé";

            public static string EncodeOrderId(int orderId)
            {
                string base36 = ConvertToBase36(orderId);

                using (var sha256 = SHA256.Create())
                {
                    string input = $"{Salt}{orderId}";
                    byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                    string hashPart = BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 6);

                    return $"{Prefix}{base36}-{hashPart}";
                }
            }

            private static string ConvertToBase36(int value)
            {
                const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var result = new StringBuilder();

                do
                {
                    result.Insert(0, chars[value % 36]);
                    value /= 36;
                } while (value > 0);

                return result.ToString();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
