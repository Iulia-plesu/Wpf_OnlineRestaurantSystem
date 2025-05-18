using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Wpf_OnlineRestaurantSystem.Models;
using Wpf_OnlineRestaurantSystem.ViewModels;

namespace Wpf_OnlineRestaurantSystem.Views
{
    public partial class DishDetailsWindow : Window
    {
        public Dish Dish { get; private set; }
        public List<AllergenSelection> AllAllergens { get; private set; }
        public List<int> SelectedAllergenIds => AllAllergens.Where(a => a.IsSelected).Select(a => a.Id).ToList();

        public DishDetailsWindow(List<Allergen> allAllergens)
        {
            InitializeComponent();
            Dish = new Dish();
            AllAllergens = allAllergens.Select(a => new AllergenSelection(a)).ToList();
            DataContext = this;
        }

        public DishDetailsWindow(Dish dishToEdit, List<Allergen> allAllergens, List<int> selectedAllergenIds)
        {
            InitializeComponent();
            Dish = new Dish
            {
                Id = dishToEdit.Id,
                Name = dishToEdit.Name,
                Description = dishToEdit.Description,
                QuantityPerPortion = dishToEdit.QuantityPerPortion,
                CurrentQuantity = dishToEdit.CurrentQuantity,
                Unit = dishToEdit.Unit,
                Price = dishToEdit.Price,
                Available = dishToEdit.Available,
                TotalQuantity = dishToEdit.TotalQuantity,
                CategoryId = dishToEdit.CategoryId,
                IsPartOfMenu = dishToEdit.IsPartOfMenu
            };

            AllAllergens = allAllergens.Select(a => new AllergenSelection(a)).ToList();

            foreach (var allergen in AllAllergens)
            {
                allergen.IsSelected = selectedAllergenIds.Contains(allergen.Id);
            }

            DataContext = this;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

    }

    public class AllergenSelection : Allergen
    {
        public bool IsSelected { get; set; }

        public AllergenSelection(Allergen allergen)
        {
            Id = allergen.Id;
            Name = allergen.Name;
        }
    }
}