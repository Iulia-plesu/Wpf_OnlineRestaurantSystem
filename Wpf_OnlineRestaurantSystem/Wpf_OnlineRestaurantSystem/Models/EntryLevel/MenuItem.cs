namespace Wpf_OnlineRestaurantSystem.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool IsMenu { get; set; }  // dacă itemul e un meniu compus


    }
}
