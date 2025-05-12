using System.Collections.Generic;
using System.Windows;
using Microsoft.Data.SqlClient;
using Wpf_OnlineRestaurantSystem.Models;
using System.Linq;
using System.Globalization;

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
                            1 AS IsMenu,
                            '0' AS TotalQuantity,
                            NULL AS Allergens
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
                            0 AS IsMenu,
                            d.TotalQuantity,
                            d.Allergens
                        FROM Dishes d
                        WHERE d.CategoryId = @CategoryId AND d.IsPartOfMenu = 0

                        UNION ALL

                        SELECT 
                            m.id AS Id,
                            m.name AS Name,
                            0.0 AS Price,
                            m.description AS Description,
                            1 AS IsMenu,
                            '0' AS TotalQuantity,
                            NULL AS Allergens
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
                        IsMenu = reader.GetInt32(4) == 1,
                        TotalQuantity = reader.IsDBNull(5) ? "0" : reader.GetString(5),
                        Allergens = reader.IsDBNull(6) ? null : reader.GetString(6)
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
                        d.Allergens,
                        d.QuantityPerPortion,
                        d.TotalQuantity
                    FROM MenuItems mi
                    INNER JOIN Dishes d ON mi.DishId = d.DishID
                    WHERE mi.MenuId = @MenuId", con);

                cmd.Parameters.AddWithValue("@MenuId", menuId);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var totalQuantity = reader.IsDBNull(6) ? "0" : reader.GetString(6);
                    var quantityPerPortion = reader.IsDBNull(5) ? "0" : reader.GetString(5);
                    var isAvailable = CalculateAvailability(totalQuantity, quantityPerPortion);

                    subItems.Add(new MenuItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Allergens = reader.IsDBNull(4) ? null : reader.GetString(4),
                        TotalQuantity = totalQuantity,
                        QuantityPerPortion = quantityPerPortion,
                        IsMenu = false,
                        IsAvailable = isAvailable
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

                var dishesCmd = new SqlCommand(@"
                    SELECT DishID, Name, Price, Description, Allergens, TotalQuantity, QuantityPerPortion
                    FROM Dishes
                    WHERE CategoryId = @CategoryId AND IsPartOfMenu = 0", con);
                dishesCmd.Parameters.AddWithValue("@CategoryId", categoryId);

                var dishesReader = dishesCmd.ExecuteReader();
                while (dishesReader.Read())
                {
                    var totalQuantity = dishesReader.IsDBNull(5) ? "0" : dishesReader.GetString(5);
                    var quantityPerPortion = dishesReader.IsDBNull(6) ? "0" : dishesReader.GetString(6);
                    var isAvailable = CalculateAvailability(totalQuantity, quantityPerPortion);

                    allItems.Add(new MenuItem
                    {
                        Id = dishesReader.GetInt32(0),
                        Name = dishesReader.GetString(1),
                        Price = dishesReader.GetDecimal(2),
                        Description = dishesReader.IsDBNull(3) ? null : dishesReader.GetString(3),
                        Allergens = dishesReader.IsDBNull(4) ? null : dishesReader.GetString(4),
                        TotalQuantity = totalQuantity,
                        QuantityPerPortion = quantityPerPortion,
                        IsMenu = false,
                        IsAvailable = isAvailable
                    });
                }
                dishesReader.Close();

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
                        TotalQuantity = "0",
                        SubItems = new List<MenuItem>(),
                        IsAvailable = true 
                    };
                    menus.Add(menu);
                }
                menuReader.Close();

                foreach (var menu in menus)
                {
                    menu.SubItems = GetSubItemsForMenu(menu.Id);
                    menu.Price = menu.SubItems.Sum(i => i.Price);

                    menu.IsAvailable = menu.SubItems.Count > 0 && menu.SubItems.All(i => i.IsAvailable);

                    allItems.Add(menu);
                }
            }

            return allItems;
        }

        private static bool CalculateAvailability(string totalQuantityStr, string quantityPerPortionStr)
        {
            if (string.IsNullOrWhiteSpace(totalQuantityStr) || string.IsNullOrWhiteSpace(quantityPerPortionStr))
                return false;

            double total = ExtractNumericValue(totalQuantityStr);
            double perPortion = ExtractNumericValue(quantityPerPortionStr);

            return total >= perPortion;
        }

        private static double ExtractNumericValue(string quantityStr)
        {
            var numericChars = quantityStr.Where(c => char.IsDigit(c) || c == '.').ToArray();
            if (numericChars.Length == 0) return 0;

            string numericString = new string(numericChars);
            if (double.TryParse(numericString, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                return value;
            }
            return 0;
        }

    }


}
