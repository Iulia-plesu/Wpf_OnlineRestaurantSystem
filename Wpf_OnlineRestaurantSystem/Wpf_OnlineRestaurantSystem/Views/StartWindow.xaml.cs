using System.Windows;

namespace Wpf_OnlineRestaurantSystem.Views
{
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow(); 
            loginWindow.Show();
            this.Close(); 
        }
        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            var menuWindow = new MenuWindow();
            menuWindow.Show();

            this.Close();
        }

    }
}
