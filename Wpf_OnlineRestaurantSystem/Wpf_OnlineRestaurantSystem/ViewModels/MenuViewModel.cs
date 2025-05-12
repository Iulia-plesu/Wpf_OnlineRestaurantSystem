using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Wpf_OnlineRestaurantSystem.Helpers;
using Wpf_OnlineRestaurantSystem.Models;
using Wpf_OnlineRestaurantSystem.Views;


namespace Wpf_OnlineRestaurantSystem.ViewModels
{
    public class MenuViewModel : INotifyPropertyChanged
    {
        public bool IsUserLoggedIn => Session.GetCurrentUserId() != -1;

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<MenuItem> MenuItems { get; set; }
        public ObservableCollection<MenuItem> SubItems { get; set; }
        public ObservableCollection<OrderItem> SelectedItems { get; set; } = new();
        

        public ICommand AddToOrderCommand { get; }

        public ICommand RemoveFromOrderCommand { get; }
        public ICommand ClearOrderCommand { get; }
        public ICommand PlaceOrderCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public decimal TotalPrice => SelectedItems.Sum(order => order.TotalPrice);

        public bool ShowOrderButtons =>
         !Session.GetCurrentUserEmail().Contains("@admin") &&
         !Session.GetCurrentUserEmail().Contains("@employee");

        private void NotifyUserStatusChanged()
        {
            OnPropertyChanged(nameof(IsUserLoggedIn));
            OnPropertyChanged(nameof(ShowOrderButtons));
        }
        public bool ShowQuantityVisibility => Session.GetCurrentUserRole() != "Customer";

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

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
                FilterMenuItems();
            }
        }
        private ObservableCollection<MenuItem> allMenuItems;

        private readonly Window currentWindow;
        public MenuViewModel(Window window)
        {
            currentWindow = window;

            var categoryList = CategoryDAL.GetAllCategories();
            categoryList.Add(new Category { Id = -1, Name = "Menus" });
            categoryList.Add(new Category { Id = -2, Name = "All Dishes" });

            Categories = new ObservableCollection<Category>(categoryList);
            MenuItems = new ObservableCollection<MenuItem>();
            SubItems = new ObservableCollection<MenuItem>();
            SelectedItems = new ObservableCollection<OrderItem>();

            AddToOrderCommand = new RelayCommand(_ => AddSelectedItem(), _ => IsUserLoggedIn);
            PlaceOrderCommand = new RelayCommand(_ => PlaceOrder(), _ => IsUserLoggedIn);
            ClearSearchCommand = new RelayCommand(_ =>
            {
                SearchText = string.Empty;
            });

            allMenuItems = new ObservableCollection<MenuItem>();

            SelectedCategory = Categories.FirstOrDefault(c => c.Name == "All Dishes") ?? Categories.FirstOrDefault();

        }
        private void LoadMenuItems()
        {
            allMenuItems.Clear();
            if (SelectedCategory == null) return;

            List<MenuItem> items;
            if (SelectedCategory.Id == -1) 
            {
                items = CategoryDAL.GetAllCategories()
                    .Where(c => c.Id > 0) 
                    .SelectMany(cat => MenuItemDAL.GetAllItemsByCategoryId(cat.Id))
                    .Where(item => item.IsMenu)
                    .ToList();
            }
            else if (SelectedCategory.Id == -2)
            {
                items = CategoryDAL.GetAllCategories()
                    .Where(c => c.Id > 0) 
                    .SelectMany(cat => MenuItemDAL.GetAllItemsByCategoryId(cat.Id))
                    .Where(item => !item.IsMenu) 
                    .ToList();
            }
            else
            {
                items = MenuItemDAL.GetAllItemsByCategoryId(SelectedCategory.Id);
            }

            foreach (var item in items)
            {
                allMenuItems.Add(item);
            }

            FilterMenuItems();
        }
        private void FilterMenuItems()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                MenuItems = new ObservableCollection<MenuItem>(allMenuItems);
            }
            else
            {
                var searchLower = SearchText.ToLower();
                var filtered = allMenuItems.Where(item =>
                    item.Name.ToLower().Contains(searchLower) ||
                    (item.IsMenu && item.SubItems?.Any(sub => sub.Name.ToLower().Contains(searchLower)) == true)
                ).ToList();

                MenuItems = new ObservableCollection<MenuItem>(filtered);
            }
            OnPropertyChanged(nameof(MenuItems));
        }
        private void PlaceOrder()
        {
            try
            {
                int currentUserId = Session.GetCurrentUserId(); 
                if (currentUserId == -1)
                {
                    MessageBox.Show("Utilizatorul nu este autentificat.");
                    return;
                }

                OrderDAL.SaveOrder(currentUserId, SelectedItems.ToList());

                MessageBox.Show("Comanda a fost plasată cu succes!");
                var statusWindow = new OrderStatusWindow();
                statusWindow.Show();
                currentWindow.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la plasarea comenzii: " + ex.Message);
            }
        }

        private void AddSelectedItem()
        {
            if (!IsUserLoggedIn)
            {
                MessageBox.Show("Trebuie să fii autentificat pentru a adăuga produse la comandă.");
                return;
            }

            if (SelectedItem == null || !SelectedItem.IsAvailable) return;

            var existingOrder = SelectedItems.FirstOrDefault(o => o.Item.Id == SelectedItem.Id);
            if (existingOrder != null)
            {
                existingOrder.Quantity++;
            }
            else
            {
                var itemToAdd = new MenuItem
                {
                    Id = SelectedItem.Id,
                    Name = SelectedItem.Name,
                    Description = SelectedItem.Description,
                    IsMenu = SelectedItem.IsMenu,
                    Price = SelectedItem.Price,
                    Allergens = SelectedItem.Allergens,
                    DiscountApplied = null,
                    IsAvailable = SelectedItem.IsAvailable
                };

                if (itemToAdd.IsMenu)
                {
                    double discount = Properties.Settings.Default.DiscountPercentage;
                    if (discount > 0)
                    {
                        itemToAdd.DiscountApplied = discount;
                        itemToAdd.Price -= itemToAdd.Price * ((decimal)discount / 100);
                    }
                }

                SelectedItems.Add(new OrderItem
                {
                    Item = itemToAdd,
                    Quantity = 1
                });
            }

            OnPropertyChanged(nameof(TotalPrice));
        }
        public void RefreshMenuItems()
        {
            if (SelectedCategory != null)
            {
                var currentSelectedId = SelectedItem?.Id;
                LoadMenuItems();

                if (currentSelectedId != null)
                {
                    SelectedItem = MenuItems.FirstOrDefault(i => i.Id == currentSelectedId);
                }
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
