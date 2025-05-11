using System.Windows;
using System.Windows.Controls;
using Wpf_OnlineRestaurantSystem.Models;

namespace Wpf_OnlineRestaurantSystem.Views
{
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset error message
            ErrorMessageText.Visibility = Visibility.Collapsed;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                string.IsNullOrWhiteSpace(AddressTextBox.Text))
            {
                ErrorMessageText.Text = "All fields are required.";
                ErrorMessageText.Visibility = Visibility.Visible;
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ErrorMessageText.Text = "Passwords do not match.";
                ErrorMessageText.Visibility = Visibility.Visible;
                return;
            }

            if (PasswordBox.Password.Length < 6)
            {
                ErrorMessageText.Text = "Password must be at least 6 characters.";
                ErrorMessageText.Visibility = Visibility.Visible;
                return;
            }

            User newUser = new User
            {
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Email = EmailTextBox.Text,
                PhoneNumber = PhoneTextBox.Text,
                Address = AddressTextBox.Text,
                Password = PasswordBox.Password
            };

            if (UserDAL.RegisterUser(newUser))
            {
                MessageBox.Show("Registration successful! You can now sign in.", "Success",
                               MessageBoxButton.OK, MessageBoxImage.Information);
                new StartWindow().Show();
                this.Close();
            }
            else
            {
                ErrorMessageText.Text = "Registration failed. Email may already be in use.";
                ErrorMessageText.Visibility = Visibility.Visible;
            }
        }

        private void LoginLink_Click(object sender, RoutedEventArgs e)
        {
            new StartWindow().Show();
            this.Close();
        }
    }
}