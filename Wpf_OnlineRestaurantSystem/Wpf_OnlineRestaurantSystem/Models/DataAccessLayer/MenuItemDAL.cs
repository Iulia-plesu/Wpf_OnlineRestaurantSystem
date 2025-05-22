using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Microsoft.Data.SqlClient;
using System.Data;

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

                SqlCommand cmd = new SqlCommand("GetItemsByCategoryId", con);
                cmd.CommandType = CommandType.StoredProcedure;
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

                SqlCommand cmd = new SqlCommand("GetSubItemsForMenu", con);
                cmd.CommandType = CommandType.StoredProcedure;
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
                        QuantityPerPortion = quantityPerPortion,
                        TotalQuantity = totalQuantity,
                        IsMenu = false,
                        IsAvailable = isAvailable,
                        ImagePath = reader.IsDBNull(7) ? null : reader.GetString(7)
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

                SqlCommand dishesCmd = new SqlCommand("GetAllItemsByCategoryId", con);
                dishesCmd.CommandType = CommandType.StoredProcedure;
                dishesCmd.Parameters.AddWithValue("@CategoryId", categoryId);

                using (var reader = dishesCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var totalQuantity = reader.IsDBNull(5) ? "0" : reader.GetString(5);
                        var quantityPerPortion = reader.IsDBNull(6) ? "0" : reader.GetString(6);
                        var isAvailable = CalculateAvailability(totalQuantity, quantityPerPortion);

                        allItems.Add(new MenuItem
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

                    if (reader.NextResult())
                    {
                        var menus = new List<MenuItem>();

                        while (reader.Read())
                        {
                            var menu = new MenuItem
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Price = 0,
                                IsMenu = true,
                                TotalQuantity = "0",
                                SubItems = new List<MenuItem>(),
                                IsAvailable = true
                            };
                            menus.Add(menu);
                        }

                        foreach (var menu in menus)
                        {
                            menu.SubItems = GetSubItemsForMenu(menu.Id);
                            menu.Price = menu.SubItems.Sum(i => i.Price);
                            menu.IsAvailable = menu.SubItems.Count > 0 && menu.SubItems.All(i => i.IsAvailable);
                            allItems.Add(menu);
                        }
                    }
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
        public static List<MenuItem> GetItemsWithoutAllergen(string allergen, int? categoryId = null)
        {
            var items = new List<MenuItem>();

            using (SqlConnection con = HelperDAL.Connection())
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("GetItemsWithoutAllergen", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Allergen", allergen);

                if (categoryId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@CategoryId", DBNull.Value);
                }

                using (var reader = cmd.ExecuteReader())
                {
                    // Read individual dishes
                    while (reader.Read())
                    {
                        var totalQuantity = reader.IsDBNull(5) ? "0" : reader.GetString(5);
                        var quantityPerPortion = reader.IsDBNull(6) ? "0" : reader.GetString(6);
                        var isAvailable = CalculateAvailability(totalQuantity, quantityPerPortion);

                        items.Add(new MenuItem
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

                    // Read menus (if available)
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            var menu = new MenuItem
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Price = 0,
                                IsMenu = true,
                                TotalQuantity = "0",
                                SubItems = new List<MenuItem>(),
                                IsAvailable = true
                            };
                            items.Add(menu);
                        }
                    }
                }
            }

            foreach (var menu in items.Where(i => i.IsMenu))
            {
                menu.SubItems = GetSubItemsForMenu(menu.Id);
                menu.Price = menu.SubItems.Sum(i => i.Price);
                menu.IsAvailable = menu.SubItems.Count > 0 && menu.SubItems.All(i => i.IsAvailable);
            }

            return items;
        }
    }
}
