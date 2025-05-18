using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Wpf_OnlineRestaurantSystem.Models;
using Wpf_OnlineRestaurantSystem.Helpers;
using Wpf_OnlineRestaurantSystem.Views;

namespace Wpf_OnlineRestaurantSystem.ViewModels
{
    public class InventoryManagementViewModel : INotifyPropertyChanged
    {
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    FilterDishes();
                }
            }
        }

        private Dish _selectedDish;
        public Dish SelectedDish
        {
            get => _selectedDish;
            set
            {
                if (_selectedDish != value)
                {
                    _selectedDish = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Dish> Dishes { get; set; }
        public ObservableCollection<Dish> AllDishes { get; set; }

        public ICommand UpdateStockCommand { get; }
        public ICommand CloseCommand { get; }

        public InventoryManagementViewModel()
        {
            Dishes = new ObservableCollection<Dish>();
            AllDishes = new ObservableCollection<Dish>();

            LoadDishes();

            UpdateStockCommand = new RelayCommand(UpdateStock);
            CloseCommand = new RelayCommand(CloseWindow);
        }

        private void LoadDishes()
        {
            var dishes = DishDAL.GetAllDishes();
            AllDishes.Clear();
            foreach (var dish in dishes)
            {
                AllDishes.Add(dish);
            }
            FilterDishes();
        }

        private void FilterDishes()
        {
            Dishes.Clear();
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                foreach (var dish in AllDishes)
                {
                    Dishes.Add(dish);
                }
            }
            else
            {
                var searchLower = SearchText.ToLower();
                foreach (var dish in AllDishes)
                {
                    if (dish.Name.ToLower().Contains(searchLower))
                    {
                        Dishes.Add(dish);
                    }
                }
            }
        }

        private void UpdateStock(object parameter)
        {
            if (SelectedDish == null || SelectedDish.QuantityToAdd <= 0) return;

            DishDAL.UpdateDishStock(SelectedDish.Id, SelectedDish.QuantityToAdd);
            LoadDishes();
            MessageBox.Show("Stock updated successfully!");
        }

        private void CloseWindow(object parameter)
        {
            Application.Current.Windows.OfType<InventoryManagementWindow>().FirstOrDefault()?.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Dish : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CurrentQuantity { get; set; }

        private int _quantityToAdd;
        public int QuantityToAdd
        {
            get => _quantityToAdd;
            set
            {
                if (_quantityToAdd != value)
                {
                    _quantityToAdd = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
