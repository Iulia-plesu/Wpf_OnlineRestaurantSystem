using System.Windows;
using Wpf_OnlineRestaurantSystem.ViewModels;
using Wpf_OnlineRestaurantSystem.Helpers;

namespace Wpf_OnlineRestaurantSystem.Views
{
    public partial class OrderStatusWindow : Window
    {
        public OrderStatusWindow()
        {
            InitializeComponent();
            DataContext = new OrderStatusViewModel(Session.GetCurrentUserId());
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Open MenuWindow and close this one
            var menuWindow = new MenuWindow();
            menuWindow.Show();
            this.Close();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Create new OrderStatusWindow before closing current one
            var newWindow = new OrderStatusWindow();


            newWindow.Show();
            this.Close();
        }
    }
}