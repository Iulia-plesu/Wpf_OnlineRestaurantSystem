using System.Windows;
using Wpf_OnlineRestaurantSystem.Helpers;
using Wpf_OnlineRestaurantSystem.ViewModels;

namespace Wpf_OnlineRestaurantSystem.Views
{
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
            DataContext = new MenuViewModel(this);
            if (Session.GetCurrentUserEmail() == "admin@admin.com")
            {
                AdminPanelBorder.Visibility = Visibility.Visible;
            }
            if (Session.GetCurrentUserEmail()  != "admin@admin.com")
            {
                OrderStatusBorder.Visibility = Visibility.Visible;
            }
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Application.Current.MainWindow = loginWindow; 
            this.Close();

        }
        private void OpenOrderStatusWindow(object sender, RoutedEventArgs e)
        {
            OrderStatusWindow orderStatusWindow = new OrderStatusWindow();
            orderStatusWindow.Show();

        }

        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();
        }
    }
}
