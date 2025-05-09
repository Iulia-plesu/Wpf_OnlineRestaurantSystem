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
    }
}
