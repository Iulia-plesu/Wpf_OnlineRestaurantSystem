using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Wpf_OnlineRestaurantSystem.Helpers;
using Wpf_OnlineRestaurantSystem.Models;
using Wpf_OnlineRestaurantSystem.Views;
using System.Linq;

namespace Wpf_OnlineRestaurantSystem.ViewModels
{
    public class MenuViewModel : INotifyPropertyChanged
    {

        private Window currentWindow;
        private Category selectedCategory;
        private MenuItem selectedItem;
        private string searchText;
        private string _allergenSearchText;
        private decimal _shippingCost = -1;
        private ObservableCollection<MenuItem> allMenuItems;


        public bool IsUserLoggedIn => Session.GetCurrentUserId() != -1;
        public bool ShowAdminButton => Session.CurrentUser?.Role == "Admin";
        public bool ShowQuantityVisibility => Session.GetCurrentUserRole() != "Customer";
        public bool ShowOrderButtons =>
            IsUserLoggedIn &&
            !Session.GetCurrentUserEmail().Contains("@admin") &&
            !Session.GetCurrentUserEmail().Contains("@employee");

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<MenuItem> MenuItems { get; set; } = new();
        public ObservableCollection<MenuItem> SubItems { get; set; } = new();
        public ObservableCollection<OrderItem> SelectedItems { get; set; } = new();

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

        public string AllergenSearchText
        {
            get => _allergenSearchText;
            set
            {
                _allergenSearchText = value;
                OnPropertyChanged();
                FilterMenuItems();
            }
        }

        public decimal ShippingCost
        {
            get
            {
                if (_shippingCost < 0)
                    _shippingCost = (decimal)Properties.Settings.Default.ShippingCost;
                return _shippingCost;
            }
            set
            {
                if (_shippingCost != value)
                {
                    _shippingCost = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal FreeShippingThreshold => (decimal)Properties.Settings.Default.FreeShippingThreshold;
        public decimal DiscountPercentage => (decimal)Properties.Settings.Default.DiscountPercentage;

        public decimal TotalPrice
        {
            get
            {
                decimal subtotal = SelectedItems.Sum(o => o.TotalPrice);
                decimal shipping = subtotal >= FreeShippingThreshold
                    ? ShippingCost * (1 - DiscountPercentage / 100m)
                    : ShippingCost;

                return subtotal + shipping;
            }
        }

        public ICommand AddToOrderCommand { get; }
        public ICommand PlaceOrderCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand ClearAllergenSearchCommand { get; }
        public ICommand OpenOrderStatusCommand { get; }


        public MenuViewModel(Window window)
        {
            currentWindow = window;

            var categoryList = CategoryDAL.GetAllCategories();
            categoryList.Add(new Category { Id = -1, Name = "Menus" });
            categoryList.Add(new Category { Id = -2, Name = "All Dishes" });

            Categories = new ObservableCollection<Category>(categoryList);
            allMenuItems = new ObservableCollection<MenuItem>();

            SelectedCategory = Categories.FirstOrDefault(c => c.Name == "All Dishes");

            AddToOrderCommand = new RelayCommand(_ => AddSelectedItem(), _ => IsUserLoggedIn);
            PlaceOrderCommand = new RelayCommand(_ => PlaceOrder(), _ => IsUserLoggedIn);
            ClearSearchCommand = new RelayCommand(_ => SearchText = string.Empty);
            ClearAllergenSearchCommand = new RelayCommand(_ => AllergenSearchText = string.Empty);
            OpenOrderStatusCommand = new RelayCommand(_ => OpenOrderStatus(), _ => IsUserLoggedIn);
        }

        public void SetWindow(Window window) => currentWindow = window;

        public void NotifyUserStatusChanged()
        {
            OnPropertyChanged(nameof(IsUserLoggedIn));
            OnPropertyChanged(nameof(ShowOrderButtons));
            OnPropertyChanged(nameof(ShowAdminButton));
        }

        public void RefreshMenuItems()
        {
            if (SelectedCategory != null)
            {
                var currentSelectedId = SelectedItem?.Id;
                LoadMenuItems();
                if (currentSelectedId != null)
                    SelectedItem = MenuItems.FirstOrDefault(i => i.Id == currentSelectedId);
            }
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
                    .Where(i => i.IsMenu)
                    .ToList();
            }
            else if (SelectedCategory.Id == -2) 
            {
                items = CategoryDAL.GetAllCategories()
                    .Where(c => c.Id > 0)
                    .SelectMany(cat => MenuItemDAL.GetAllItemsByCategoryId(cat.Id))
                    .Where(i => !i.IsMenu)
                    .ToList();
            }
            else 
            {
                items = MenuItemDAL.GetAllItemsByCategoryId(SelectedCategory.Id);
            }

            foreach (var item in items)
            {
                item.ImagePath = item.Id switch
                {
                    105 => @"C:\...\Images\papanasi.jpeg",
                    28 => @"C:\...\Images\chiftele.jpeg",
                    29 => @"C:\...\Images\bruschete.jpeg",
                    30 => @"C:\...\Images\platouTraditional.jpeg",
                    19 => @"C:\...\Images\ciorbaDeBurta.jpeg",
                    47 => @"C:\...\Images\tiramisu.jpeg",
                    _ => item.ImagePath
                };

                allMenuItems.Add(item);
            }

            FilterMenuItems();
        }

        private void FilterMenuItems()
        {
            IEnumerable<MenuItem> filtered = allMenuItems;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string query = SearchText.ToLower();
                filtered = filtered.Where(item =>
                    item.Name.ToLower().Contains(query) ||
                    (item.IsMenu && item.SubItems?.Any(sub => sub.Name.ToLower().Contains(query)) == true));
            }

            if (!string.IsNullOrWhiteSpace(AllergenSearchText))
            {
                string allergen = AllergenSearchText.ToLower();
                filtered = filtered.Where(item =>
                {
                    if (item.IsMenu)
                        return item.SubItems.All(sub => string.IsNullOrEmpty(sub.Allergens) || !sub.Allergens.ToLower().Contains(allergen));
                    return string.IsNullOrEmpty(item.Allergens) || !item.Allergens.ToLower().Contains(allergen);
                });
            }

            MenuItems = new ObservableCollection<MenuItem>(filtered);
            OnPropertyChanged(nameof(MenuItems));
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

        private void AddSelectedItem()
        {
            if (!IsUserLoggedIn)
            {
                MessageBox.Show("Trebuie să fii autentificat pentru a adăuga produse la comandă.");
                return;
            }

            if (SelectedItem == null || !SelectedItem.IsAvailable) return;

            var existing = SelectedItems.FirstOrDefault(o => o.Item.Id == SelectedItem.Id);
            if (existing != null)
            {
                existing.Quantity++;
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

                SelectedItems.Add(new OrderItem { Item = itemToAdd, Quantity = 1 });
            }

            OnPropertyChanged(nameof(TotalPrice));
        }

        private void PlaceOrder()
        {
            try
            {
                int userId = Session.GetCurrentUserId();
                if (userId == -1)
                {
                    MessageBox.Show("Utilizatorul nu este autentificat.");
                    return;
                }

                decimal subtotal = SelectedItems.Sum(i => i.TotalPrice);
                decimal shippingCost = subtotal >= FreeShippingThreshold
                    ? ShippingCost * (1 - DiscountPercentage / 100m)
                    : ShippingCost;

                ShippingCost = shippingCost;
                OrderDAL.SaveOrder(userId, SelectedItems.ToList(), shippingCost);

                MessageBox.Show("Comanda a fost plasată cu succes!");
                new OrderStatusWindow().Show();
                currentWindow.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la plasarea comenzii: " + ex.Message);
            }
        }

        private void OpenOrderStatus() => new OrderStatusWindow().Show();


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
