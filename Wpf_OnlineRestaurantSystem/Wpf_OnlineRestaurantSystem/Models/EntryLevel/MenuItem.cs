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
    }
}
