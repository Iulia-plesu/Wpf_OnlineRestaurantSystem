using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Wpf_OnlineRestaurantSystem.Models;

namespace Wpf_OnlineRestaurantSystem.Models
{
    public class MenuItemDAL
    {
        public static List<MenuItem> GetAllMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using (SqlConnection con = HelperDAL.Connection)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, Name, Price FROM MenuItems", con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    menuItems.Add(new MenuItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2)
                    });
                }
            }

            return menuItems;
        }
    }
}
