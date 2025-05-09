using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Wpf_OnlineRestaurantSystem.Models;
using Wpf_OnlineRestaurantSystem.Views;
using Wpf_OnlineRestaurantSystem.Helpers;
using System.Linq;


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
                // Salvează utilizatorul în sesiune
                Helpers.Session.CurrentUser = user;

                Message = $"Bun venit, {user.FirstName}!";
                

                // Deschide fereastra principală (MenuWindow)
                var menuWindow = new MenuWindow();
                menuWindow.Show();

                // Închide fereastra curentă (LoginWindow)
                Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w is LoginWindow)?
                    .Close();
            }
            else
            {
                Message = "Email sau parolă incorectă.";
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
