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
            -- Get dishes directly in the selected category
            SELECT 
                d.DishID AS Id,
                d.Name,
                d.Price,
                d.Description,
                0 AS IsMenu
            FROM Dishes d
            WHERE d.CategoryId = @CategoryId

            UNION

            -- Get menus that include at least one dish from this category
            SELECT 
                DISTINCT m.id AS Id,
                m.name AS Name,
                0.0 AS Price, -- You can calculate menu price elsewhere
                m.description AS Description,
                1 AS IsMenu
            FROM Menus m
            INNER JOIN MenuItems mi ON m.id = mi.MenuId
            INNER JOIN Dishes d ON mi.DishId = d.DishID
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

                // Feluri de mâncare simple
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

                // Meniuri
                var menuCmd = new SqlCommand(@"
            SELECT m.Id, m.Name, m.Description, m.DiscountPercentage
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
                        Price = 0,  // îl calculăm mai jos din subitems
                        IsMenu = true,
                        SubItems = new List<MenuItem>()
                    };

                    menus.Add(menu);
                }
                menuReader.Close();

                // Adaugă sub-itemi și calculează prețul meniului (opțional)
                foreach (var menu in menus)
                {
                    menu.SubItems = GetSubItemsForMenu(menu.Id);

                    // Poți calcula un preț total cu discount:
                    decimal totalPrice = menu.SubItems.Sum(i => i.Price);
                    decimal discount = GetMenuDiscount(menu.Id, con);
                    menu.Price = totalPrice * (1 - discount / 100);

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
