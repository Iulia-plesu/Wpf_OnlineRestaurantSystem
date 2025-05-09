using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Wpf_OnlineRestaurantSystem.Models;


namespace Wpf_OnlineRestaurantSystem.ViewModels
{
    public class MenuViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<MenuItem> MenuItems { get; set; }
        public ObservableCollection<MenuItem> SubItems { get; set; }

        private Category selectedCategory;
        public Category SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                OnPropertyChanged();
                LoadMenuItems();
            }
        }

        private MenuItem selectedItem;
        public MenuItem SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
                LoadSubItemsOrDetails();
            }
        }

        public MenuViewModel()
        {
            var categoryList = CategoryDAL.GetAllCategories();

            categoryList.Add(new Category { Id = -1, Name = "Menus" });

            Categories = new ObservableCollection<Category>(categoryList);
            MenuItems = new ObservableCollection<MenuItem>();
            SubItems = new ObservableCollection<MenuItem>();
        }

        private void LoadMenuItems()
        {
            MenuItems.Clear();
            if (SelectedCategory == null) return;
            if (SelectedCategory.Id == -1)
            {
                var allMenus = CategoryDAL.GetAllCategories()
                    .SelectMany(cat => MenuItemDAL.GetAllItemsByCategoryId(cat.Id))
                    .Where(item => item.IsMenu);

                foreach (var menu in allMenus)
                    MenuItems.Add(menu);
            }
            else
            {
                foreach (var item in MenuItemDAL.GetAllItemsByCategoryId(SelectedCategory.Id))
                    MenuItems.Add(item);
            }

        }

        private void LoadSubItemsOrDetails()
        {
            SubItems.Clear();
            if (SelectedItem?.IsMenu == true)
            {
                foreach (var sub in MenuItemDAL.GetSubItemsForMenu(SelectedItem.Id))
                    SubItems.Add(sub);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
