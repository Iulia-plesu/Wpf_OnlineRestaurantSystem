using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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

        public AdminViewModel()
        {
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
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
