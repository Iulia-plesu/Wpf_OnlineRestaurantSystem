using System;
using System.Collections.Generic;
using System.Data;
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
                // 1. Inserează în tabela Orders
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
                    var itemCmd = new SqlCommand(@"
                        INSERT INTO OrderItems (OrderID, DishID, Quantity, UnitPrice)
                        VALUES (@OrderID, @DishID, @Quantity, @UnitPrice)", con, transaction);

                    itemCmd.Parameters.AddWithValue("@OrderID", orderId);
                    itemCmd.Parameters.AddWithValue("@DishID", item.Item.Id); 
                    itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    itemCmd.Parameters.AddWithValue("@UnitPrice", item.Item.Price);

                    itemCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Eroare la salvarea comenzii: " + ex.Message);
            }
        }
    }
}
