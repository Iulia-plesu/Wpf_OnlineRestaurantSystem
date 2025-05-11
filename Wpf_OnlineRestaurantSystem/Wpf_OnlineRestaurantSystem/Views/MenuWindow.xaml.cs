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
    }
}
