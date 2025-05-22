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
        public ICommand OpenOrderStatusCommand { get; }
        public ICommand RemoveFromOrderCommand { get; }
        public ICommand ClearOrderCommand { get; }
        public ICommand PlaceOrderCommand { get; }
        public ICommand ClearSearchCommand { get; }

        private decimal _shippingCost = -1;

        public decimal ShippingCost
        {
            get
            {
                if (_shippingCost < 0)
                {
                    _shippingCost = (decimal)Properties.Settings.Default.ShippingCost;
                }
                return _shippingCost;
            }
            set
            {
                if (_shippingCost != value)
                {
                    _shippingCost = value;
                    OnPropertyChanged(nameof(ShippingCost));
                }
            }
        }


        public decimal FreeShippingThreshold => (decimal)Properties.Settings.Default.FreeShippingThreshold;
        public decimal DiscountPercentage => (decimal)Properties.Settings.Default.DiscountPercentage;

        public decimal TotalPrice
        {
            get
            {
                decimal subtotal = SelectedItems.Sum(order => order.TotalPrice);
                decimal shipping = ShippingCost;

                if (subtotal >= FreeShippingThreshold)
                {
                    shipping = ShippingCost * (1 - (DiscountPercentage / 100m));
                    ShippingCost = shipping;
                    OnPropertyChanged(nameof(ShippingCost));
                }

                return subtotal + shipping;
            }
        }

        public bool ShowAdminButton => Helpers.Session.CurrentUser?.Role == "Admin";


        public bool ShowOrderButtons =>
            IsUserLoggedIn &&
            !Session.GetCurrentUserEmail().Contains("@admin") &&
            !Session.GetCurrentUserEmail().Contains("@employee");

        public void NotifyUserStatusChanged()
        {
            OnPropertyChanged(nameof(IsUserLoggedIn));
            OnPropertyChanged(nameof(ShowOrderButtons));
            OnPropertyChanged(nameof(ShowAdminButton));
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
        private string _allergenSearchText;
        public string AllergenSearchText
        {
            get => _allergenSearchText;
            set
            {
                _allergenSearchText = value;
                OnPropertyChanged(nameof(AllergenSearchText));
                FilterMenuItems();
            }
        }
        public ICommand ClearAllergenSearchCommand { get; }

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

        private Window currentWindow;
        public void SetWindow(Window window) => currentWindow = window;
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
            ClearAllergenSearchCommand = new RelayCommand(_ =>
            {
                AllergenSearchText = string.Empty;
            });
            OpenOrderStatusCommand = new RelayCommand(_ => OpenOrderStatus(), _ => IsUserLoggedIn);

            allMenuItems = new ObservableCollection<MenuItem>();

            SelectedCategory = Categories.FirstOrDefault(c => c.Name == "All Dishes") ?? Categories.FirstOrDefault();

        }
        private void FilterMenuItems()
        {
            IEnumerable<MenuItem> filtered = allMenuItems;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchLower = SearchText.ToLower();
                filtered = filtered.Where(item =>
                    item.Name.ToLower().Contains(searchLower) ||
                    (item.IsMenu && item.SubItems?.Any(sub => sub.Name.ToLower().Contains(searchLower)) == true)
                );
            }

            if (!string.IsNullOrWhiteSpace(AllergenSearchText))
            {
                var allergenLower = AllergenSearchText.ToLower();
                filtered = filtered.Where(item =>
                {
                    if (item.IsMenu)
                    {
                        return item.SubItems.All(sub =>
                            string.IsNullOrEmpty(sub.Allergens) ||
                            !sub.Allergens.ToLower().Contains(allergenLower));
                    }
                    else
                    {
                        return string.IsNullOrEmpty(item.Allergens) ||
                               !item.Allergens.ToLower().Contains(allergenLower);
                    }
                });
            }

            MenuItems = new ObservableCollection<MenuItem>(filtered);
            OnPropertyChanged(nameof(MenuItems));
        }
        private void OpenOrderStatus()
        {
            var statusWindow = new OrderStatusWindow();
            statusWindow.Show();
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
                if (item.Id == 105)
                    item.ImagePath = @"C:\Users\Plesu\Desktop\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Images\papanasi.jpeg";
                if (item.Id == 28)
                    item.ImagePath = @"C:\Users\Plesu\Desktop\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Images\chiftele.jpeg";
                if (item.Id == 29)
                    item.ImagePath = @"C:\Users\Plesu\Desktop\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Images\bruschete.jpeg";
                if (item.Id == 30)
                    item.ImagePath = @"C:\Users\Plesu\Desktop\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Images\platouTraditional.jpeg";
                if (item.Id == 19)
                    item.ImagePath = @"C:\Users\Plesu\Desktop\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Images\ciorbaDeBurta.jpeg";
                if (item.Id == 47)
                    item.ImagePath = @"C:\Users\Plesu\Desktop\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Wpf_OnlineRestaurantSystem\Images\tiramisu.jpeg";
                

                allMenuItems.Add(item);
            }
            

            FilterMenuItems();
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

                decimal subtotal = SelectedItems.Sum(item => item.TotalPrice);
                decimal shippingCost = ShippingCost;

                if (subtotal >= FreeShippingThreshold)
                {
                    shippingCost = ShippingCost * (1 - (DiscountPercentage / 100m));
                    ShippingCost = shippingCost;
                    OnPropertyChanged(nameof(ShippingCost));
                }

                OrderDAL.SaveOrder(currentUserId, SelectedItems.ToList(), shippingCost);

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
