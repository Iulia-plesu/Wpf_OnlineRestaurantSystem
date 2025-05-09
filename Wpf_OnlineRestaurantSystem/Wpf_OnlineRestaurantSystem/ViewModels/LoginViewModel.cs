using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Wpf_OnlineRestaurantSystem.Models;

namespace Wpf_OnlineRestaurantSystem.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string email;
        private string password;
        private string message;

        public string Email
        {
            get => email;
            set { email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => password;
            set { password = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get => message;
            set { message = value; OnPropertyChanged(); }
        }

        public RelayCommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login(object parameter)
        {
            var user = UserDAL.GetUserByEmailAndPassword(Email, Password);
            if (user != null)
            {
                Message = $"Bun venit, {user.FirstName}!";
                MessageBox.Show(Message);  // Sau, poți să folosești o metodă mai elegantă pentru a afișa un mesaj
                                           // Navighează spre fereastra principală
            }
            else
            {
                Message = "Email sau parolă incorectă.";
                MessageBox.Show(Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
