using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf_OnlineRestaurantSystem.Models;
using Wpf_OnlineRestaurantSystem.ViewModels;
using Wpf_OnlineRestaurantSystem.Helpers;

namespace Wpf_OnlineRestaurantSystem.Views
{
    /// <summary>
    /// Interaction logic for OrderStatusWindow.xaml
    /// </summary>
    public partial class OrderStatusWindow : Window
    {
        public OrderStatusWindow()
        {
            InitializeComponent();
            DataContext = new OrderStatusViewModel(Session.GetCurrentUserId());
        }
    }
}
