using System.Windows;
using Wpf_OnlineRestaurantSystem.ViewModels;

namespace Wpf_OnlineRestaurantSystem.Views
{
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
            DataContext = new MenuViewModel(this);
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

    }
}
