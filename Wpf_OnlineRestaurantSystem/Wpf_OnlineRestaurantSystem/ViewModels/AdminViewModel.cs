using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Wpf_OnlineRestaurantSystem.Models;

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
        public ICommand UpdateOrderStatusCommand { get; }

        public ICommand CancelOrderCommand { get; }
        public AdminViewModel()
        {
            CancelOrderCommand = new RelayCommand(CancelOrder);
            UpdateOrderStatusCommand = new RelayCommand(UpdateOrderStatus);

            LoadUsersAndOrders();
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
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
