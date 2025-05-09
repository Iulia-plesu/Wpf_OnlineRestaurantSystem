using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;
using Wpf_OnlineRestaurantSystem.Models;

namespace Wpf_OnlineRestaurantSystem.Models
{
    public static class MenuItemDAL
    {
        public static List<MenuItem> GetItemsByCategoryId(int categoryId)
        {
            var items = new List<MenuItem>();
            using (SqlConnection con = HelperDAL.Connection())
            {
                con.Open();
                var cmd = new SqlCommand(@"
                    SELECT 
                        d.DishID,
                        d.Name,
                        d.Price,
                        d.Description,
                        0 AS IsMenu
                    FROM Dishes d
                    WHERE d.CategoryId = @CategoryId", con);


                cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new MenuItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                        IsMenu = reader.GetInt32(4) == 1
                    });
                }

            }
            return items;
        }


        public static List<MenuItem> GetSubItemsForMenu(int menuId)
        {
            var subItems = new List<MenuItem>();
            using (SqlConnection con = HelperDAL.Connection())
            {
                con.Open();
                var cmd = new SqlCommand("SELECT Id, Name, Price, Description, IsMenu FROM MenuItems WHERE ParentMenuId = @MenuId", con);
                cmd.Parameters.AddWithValue("@MenuId", menuId);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    subItems.Add(new MenuItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                        IsMenu = reader.GetBoolean(4)
                    });
                }
            }
            return subItems;
        }
    }
}
