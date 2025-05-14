using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_OnlineRestaurantSystem.Models
{
    public class UserWithOrders
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public List<UserOrder> Orders { get; set; }
    }

}
