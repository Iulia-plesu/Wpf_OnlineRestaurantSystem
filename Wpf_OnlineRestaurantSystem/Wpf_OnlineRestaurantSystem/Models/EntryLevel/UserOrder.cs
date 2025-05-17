using System;
using System.Collections.Generic;

namespace Wpf_OnlineRestaurantSystem.Models
{
    public class UserOrder
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<string> Items { get; set; } = new List<string>();
    
}
}
