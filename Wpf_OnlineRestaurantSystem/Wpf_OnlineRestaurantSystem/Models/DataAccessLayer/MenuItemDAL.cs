using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;
using Wpf_OnlineRestaurantSystem.Models;
using System.Linq;

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

                SqlCommand cmd;

                if (categoryId == -1)
                {
                    cmd = new SqlCommand(@"
                        SELECT 
                            m.id AS Id,
                            m.name AS Name,
                            0.0 AS Price,
                            m.description AS Description,
                            1 AS IsMenu
                        FROM Menus m", con);
                }
                else
                {
                    cmd = new SqlCommand(@"
                        SELECT 
                            d.DishID AS Id,
                            d.Name,
                            d.Price,
                            d.Description,
                            0 AS IsMenu
                        FROM Dishes d
                        WHERE d.CategoryId = @CategoryId

                        UNION ALL

                        SELECT 
                            m.id AS Id,
                            m.name AS Name,
                            0.0 AS Price,
                            m.description AS Description,
                            1 AS IsMenu
                        FROM Menus m
                        WHERE m.CategoryId = @CategoryId", con);

                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                }

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
                var cmd = new SqlCommand(@"
                    SELECT 
                        d.DishID,
                        d.Name,
                        d.Price,
                        d.Description,
                        0 AS IsMenu,
                        d.Allergens
                    FROM MenuItems mi
                    INNER JOIN Dishes d ON mi.DishId = d.DishID
                    WHERE mi.MenuId = @MenuId", con);

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
                        IsMenu = false,
                        Allergens = reader.IsDBNull(5) ? null : reader.GetString(5)
                    });
                }
            }

            return subItems;
        }

        public static List<MenuItem> GetAllItemsByCategoryId(int categoryId)
        {
            var allItems = new List<MenuItem>();

            using (var con = HelperDAL.Connection())
            {
                con.Open();

                // Select dishes
                var dishesCmd = new SqlCommand(@"
            SELECT DishID, Name, Price, Description, Allergens
            FROM Dishes
            WHERE CategoryId = @CategoryId", con);
                dishesCmd.Parameters.AddWithValue("@CategoryId", categoryId);

                var reader = dishesCmd.ExecuteReader();
                while (reader.Read())
                {
                    allItems.Add(new MenuItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Allergens = reader.IsDBNull(4) ? null : reader.GetString(4),
                        IsMenu = false
                    });
                }
                reader.Close();

                // Select menus (fără DiscountPercentage)
                var menuCmd = new SqlCommand(@"
            SELECT m.Id, m.Name, m.Description
            FROM Menus m
            WHERE m.CategoryId = @CategoryId", con);
                menuCmd.Parameters.AddWithValue("@CategoryId", categoryId);

                var menuReader = menuCmd.ExecuteReader();
                var menus = new List<MenuItem>();

                while (menuReader.Read())
                {
                    var menu = new MenuItem
                    {
                        Id = menuReader.GetInt32(0),
                        Name = menuReader.GetString(1),
                        Description = menuReader.IsDBNull(2) ? null : menuReader.GetString(2),
                        Price = 0,
                        IsMenu = true,
                        SubItems = new List<MenuItem>()
                    };

                    menus.Add(menu);
                }
                menuReader.Close();

                foreach (var menu in menus)
                {
                    menu.SubItems = GetSubItemsForMenu(menu.Id);
                    menu.Price = menu.SubItems.Sum(i => i.Price); // fără discount

                    allItems.Add(menu);
                }
            }

            return allItems;
        }

        private static decimal GetMenuDiscount(int menuId, SqlConnection con)
        {
            var cmd = new SqlCommand("SELECT DiscountPercentage FROM Menus WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", menuId);
            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToDecimal(result) : 0;
        }
    }
}
