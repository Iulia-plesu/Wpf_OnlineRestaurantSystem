using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace Wpf_OnlineRestaurantSystem.Models
{
    public static class OrderDAL
    {
        public static void SaveOrder(int userId, List<OrderItem> items)
        {
            using var con = HelperDAL.Connection();
            con.Open();

            using var transaction = con.BeginTransaction();

            try
            {
                var orderCmd = new SqlCommand(@"
                    INSERT INTO Orders (UserID, OrderDate, Status, TotalAmount)
                    OUTPUT INSERTED.OrderID
                    VALUES (@UserID, GETDATE(), @Status, @TotalAmount)", con, transaction);

                orderCmd.Parameters.AddWithValue("@UserID", userId);
                orderCmd.Parameters.AddWithValue("@Status", "Pending");
                orderCmd.Parameters.AddWithValue("@TotalAmount", items.Sum(i => i.TotalPrice));

                int orderId = (int)orderCmd.ExecuteScalar();

                foreach (var item in items)
                {
                    if (!item.Item.IsMenu)
                    {
                        var itemCmd = new SqlCommand(@"
                            INSERT INTO OrderItems (OrderID, DishID, Quantity, UnitPrice)
                            VALUES (@OrderID, @DishID, @Quantity, @UnitPrice)", con, transaction);

                        itemCmd.Parameters.AddWithValue("@OrderID", orderId);
                        itemCmd.Parameters.AddWithValue("@DishID", item.Item.Id);
                        itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        itemCmd.Parameters.AddWithValue("@UnitPrice", item.Item.Price);
                        itemCmd.ExecuteNonQuery();

                        ScadeCantitatiIngredient(item.Item.Id, item.Quantity, con, transaction);
                    }
                    else
                    {
                        var getMenuItemsCmd = new SqlCommand(@"
                            SELECT DishID FROM MenuItems WHERE MenuID = @MenuID", con, transaction);
                        getMenuItemsCmd.Parameters.AddWithValue("@MenuID", item.Item.Id);

                        var dishIdsInMenu = new List<int>();
                        using (var reader = getMenuItemsCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dishIdsInMenu.Add((int)reader["DishID"]);
                            }
                        }

                        foreach (var dishId in dishIdsInMenu)
                        {
                            var subDishCmd = new SqlCommand(@"
                                INSERT INTO OrderItems (OrderID, DishID, Quantity, UnitPrice)
                                SELECT @OrderID, DishID, @Quantity, Price
                                FROM Dishes WHERE DishID = @DishID", con, transaction);

                            subDishCmd.Parameters.AddWithValue("@OrderID", orderId);
                            subDishCmd.Parameters.AddWithValue("@DishID", dishId);
                            subDishCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                            subDishCmd.ExecuteNonQuery();

                            ScadeCantitatiIngredient(dishId, item.Quantity, con, transaction);
                        }
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Eroare la salvarea comenzii: " + ex.Message);
            }
        }
        private static void ScadeCantitatiIngredient(int dishId, int multiplier, SqlConnection con, SqlTransaction transaction)
        {
            var getPortionCmd = new SqlCommand(@"
                SELECT QuantityPerPortion, TotalQuantity
                FROM Dishes
                WHERE DishID = @DishID", con, transaction);
            getPortionCmd.Parameters.AddWithValue("@DishID", dishId);

            using var reader = getPortionCmd.ExecuteReader();
            if (reader.Read())
            {
                string quantityPerPortion = reader["QuantityPerPortion"]?.ToString();
                string totalQuantity = reader["TotalQuantity"]?.ToString();

                if (!string.IsNullOrWhiteSpace(quantityPerPortion) &&
                    !string.IsNullOrWhiteSpace(totalQuantity))
                {
                    string totalToSubtract = MultiplyQuantityString(quantityPerPortion, multiplier);
                    string updatedTotal = ScadeCantitate(totalQuantity, totalToSubtract);

                    reader.Close(); 

                    var updateQtyCmd = new SqlCommand(@"
                UPDATE Dishes
                SET TotalQuantity = @UpdatedTotal
                WHERE DishID = @DishID", con, transaction);

                    updateQtyCmd.Parameters.AddWithValue("@UpdatedTotal", updatedTotal);
                    updateQtyCmd.Parameters.AddWithValue("@DishID", dishId);

                    updateQtyCmd.ExecuteNonQuery();
                }
            }
        }

        private static string ScadeCantitate(string totalQuantity, string toSubtract)
        {
            string unitTotal = new string(totalQuantity.Where(char.IsLetter).ToArray());
            string unitSubtract = new string(toSubtract.Where(char.IsLetter).ToArray());

            if (unitTotal != unitSubtract)
                return totalQuantity; 

            string totalStr = totalQuantity.Replace(unitTotal, "").Trim();
            string subtractStr = toSubtract.Replace(unitSubtract, "").Trim();

            if (double.TryParse(totalStr, out double totalVal) &&
                double.TryParse(subtractStr, out double subVal))
            {
                double result = Math.Max(totalVal - subVal, 0);
                return $"{result}{unitTotal}";
            }

            return totalQuantity;
        }

        private static string MultiplyQuantityString(string quantityStr, int multiplier)
        {
            string unit = new string(quantityStr.Where(char.IsLetter).ToArray());
            string valueStr = quantityStr.Replace(unit, "").Trim();

            if (double.TryParse(valueStr, out double value))
            {
                double result = value * multiplier;
                return $"{result}{unit}";
            }

            return quantityStr;
        }

    }
}
