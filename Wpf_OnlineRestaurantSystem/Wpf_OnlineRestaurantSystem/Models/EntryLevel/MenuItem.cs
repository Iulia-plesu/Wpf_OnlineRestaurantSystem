using Wpf_OnlineRestaurantSystem.ViewModels;

namespace Wpf_OnlineRestaurantSystem.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool IsMenu { get; set; }  
        public string? Allergens { get; set; }

        public List<MenuItem>? SubItems { get; set; }
        public double? DiscountApplied { get; set; }
        public string TotalQuantity { get; set; }
        public bool IsAvailable { get; set; }
        public string QuantityPerPortion { get; set; }

        public Dish Dish { get; set; }

        public MenuItem()
        {
            Dish = new Dish(); 
        }
        public string ImagePath
        {
            get => Dish?.ImagePath;
            set
            {
                if (Dish == null) Dish = new Dish();
                Dish.ImagePath = value;
            }
        }
    }
}
