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
        private void NotifyUserStatusChanged()
        {
            OnPropertyChanged(nameof(IsUserLoggedIn));
        }

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<MenuItem> MenuItems { get; set; }
        public ObservableCollection<MenuItem> SubItems { get; set; }
        public ObservableCollection<OrderItem> SelectedItems { get; set; } = new();
        

        public ICommand AddToOrderCommand { get; }

        public ICommand RemoveFromOrderCommand { get; }
        public ICommand ClearOrderCommand { get; }
        public ICommand PlaceOrderCommand { get; }
        public decimal TotalPrice => SelectedItems.Sum(order => order.TotalPrice);


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
        private readonly Window currentWindow;
        public MenuViewModel(Window window)
        {
            currentWindow = window;

            var categoryList = CategoryDAL.GetAllCategories();
            categoryList.Add(new Category { Id = -1, Name = "Menus" });

            Categories = new ObservableCollection<Category>(categoryList);
            MenuItems = new ObservableCollection<MenuItem>();
            SubItems = new ObservableCollection<MenuItem>();
            SelectedItems = new ObservableCollection<OrderItem>();

            AddToOrderCommand = new RelayCommand(_ => AddSelectedItem(), _ => IsUserLoggedIn);
            PlaceOrderCommand = new RelayCommand(_ => PlaceOrder(), _ => IsUserLoggedIn);


            SelectedCategory = Categories.FirstOrDefault(c => c.Name == "Menus") ?? Categories.FirstOrDefault();

        }
        private void PlaceOrder()
        {
            try
            {
                int currentUserId = Session.GetCurrentUserId(); // Acces corect la ID-ul userului
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

            if (SelectedItem == null) return;

            // Check if item already exists in the order
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
                    DiscountApplied = null
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
