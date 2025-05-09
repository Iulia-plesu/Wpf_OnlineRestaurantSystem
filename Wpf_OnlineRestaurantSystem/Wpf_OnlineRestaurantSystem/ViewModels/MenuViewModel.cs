using System.Collections.ObjectModel;
using Wpf_OnlineRestaurantSystem.Models;


namespace Wpf_OnlineRestaurantSystem.ViewModels
{
    public class MenuViewModel
    {
        public ObservableCollection<Category> Categories { get; set; }

        public MenuViewModel()
        {
            Categories = new ObservableCollection<Category>(CategoryDAL.GetAllCategories());
        }
    }
}
