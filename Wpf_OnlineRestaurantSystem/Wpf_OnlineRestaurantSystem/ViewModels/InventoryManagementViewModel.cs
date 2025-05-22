using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
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
        public List<Allergen> AllAllergens { get; set; }

        public ICommand UpdateStockCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand AddNewDishCommand { get; }
        public ICommand EditDishCommand { get; }
        public ICommand DeleteDishCommand { get; }

        public InventoryManagementViewModel()
        {
            Dishes = new ObservableCollection<Dish>();
            AllDishes = new ObservableCollection<Dish>();
            AllAllergens = DishDAL.GetAllAllergens();

            LoadDishes();

            UpdateStockCommand = new RelayCommand(UpdateStock);
            CloseCommand = new RelayCommand(CloseWindow);
            AddNewDishCommand = new RelayCommand(AddNewDish);
            EditDishCommand = new RelayCommand(EditDish);
            DeleteDishCommand = new RelayCommand(DeleteDish);
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

        private void AddNewDish(object parameter)
        {
            var dialog = new DishDetailsWindow(AllAllergens);
            if (dialog.ShowDialog() == true)
            {
                var newDish = dialog.Dish;
                var selectedAllergenIds = dialog.SelectedAllergenIds;
                DishDAL.CreateDish(newDish, selectedAllergenIds);
                LoadDishes();
                MessageBox.Show("Dish created successfully!");
            }
        }

        private void EditDish(object parameter)
        {
            if (SelectedDish == null)
            {
                MessageBox.Show("Please select a dish to edit.");
                return;
            }

            var currentAllergens = DishDAL.GetDishAllergens(SelectedDish.Id);
            var dialog = new DishDetailsWindow(SelectedDish, AllAllergens, currentAllergens.Select(a => a.Id).ToList());

            if (dialog.ShowDialog() == true)
            {
                var updatedDish = dialog.Dish;
                var selectedAllergenIds = dialog.SelectedAllergenIds;
                DishDAL.UpdateDish(updatedDish, selectedAllergenIds);
                LoadDishes();
                MessageBox.Show("Dish updated successfully!");
            }
        }

        private void DeleteDish(object parameter)
        {
            if (SelectedDish == null)
            {
                MessageBox.Show("Please select a dish to delete.");
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{SelectedDish.Name}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                DishDAL.DeleteDish(SelectedDish.Id);
                LoadDishes();
                MessageBox.Show("Dish deleted successfully!");
            }
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
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public string QuantityPerPortion { get; set; }
        public string TotalQuantity { get; set; } 
        public int? CategoryId { get; set; } 
        public bool IsPartOfMenu { get; set; }
        public string ImagePath { get; set; }  


        public string CurrentQuantity
        {
            get => TotalQuantity;
            set => TotalQuantity = value;
        }

        public string Unit { get; set; } = "";

        private string _allergens;
        public string Allergens
        {
            get => _allergens;
            set
            {
                if (_allergens != value)
                {
                    _allergens = value;
                    OnPropertyChanged();
                }
            }
        }

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