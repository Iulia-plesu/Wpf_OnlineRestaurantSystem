﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace Wpf_OnlineRestaurantSystem.Models
{
    public static class OrderDAL
    {
        public static void SaveOrder(int userId, List<OrderItem> items, decimal shippingCost)
        {
            using var con = HelperDAL.Connection();
            con.Open();

            using var transaction = con.BeginTransaction();
            try
            {
                var itemTable = new DataTable();
                itemTable.Columns.Add("DishID", typeof(int));
                itemTable.Columns.Add("Quantity", typeof(int));
                itemTable.Columns.Add("UnitPrice", typeof(decimal));

                foreach (var item in items)
                {
                    if (!item.Item.IsMenu)
                    {
                        itemTable.Rows.Add(item.Item.Id, item.Quantity, item.Item.Price);
                    }
                    else
                    {
                        var getMenuItemsCmd = new SqlCommand("GetMenuItems", con, transaction)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        getMenuItemsCmd.Parameters.AddWithValue("@MenuID", item.Item.Id);

                        var dishIds = new List<int>();
                        using (var reader = getMenuItemsCmd.ExecuteReader())
                        {
                            while (reader.Read())
                                dishIds.Add(reader.GetInt32(0));
                        }

                        foreach (var dishId in dishIds)
                        {
                            var getPriceCmd = new SqlCommand("GetDishPrice", con, transaction)
                            {
                                CommandType = CommandType.StoredProcedure
                            };
                            getPriceCmd.Parameters.AddWithValue("@DishID", dishId);

                            var priceParam = new SqlParameter("@Price", SqlDbType.Decimal)
                            {
                                Direction = ParameterDirection.Output,
                                Precision = 10,
                                Scale = 2
                            };
                            getPriceCmd.Parameters.Add(priceParam);

                            getPriceCmd.ExecuteNonQuery();

                            decimal price = (decimal)priceParam.Value;

                            itemTable.Rows.Add(dishId, item.Quantity, price);
                        }
                    }
                }

                var saveOrderCmd = new SqlCommand("SaveOrder", con, transaction)
                {
                    CommandType = CommandType.StoredProcedure
                };
                saveOrderCmd.Parameters.AddWithValue("@UserID", userId);
                saveOrderCmd.Parameters.AddWithValue("@TotalAmount", items.Sum(i => i.TotalPrice));

                var itemsParam = saveOrderCmd.Parameters.AddWithValue("@OrderItems", itemTable);
                itemsParam.SqlDbType = SqlDbType.Structured;
                itemsParam.TypeName = "OrderItemTableType";

                saveOrderCmd.ExecuteNonQuery();

                foreach (DataRow row in itemTable.Rows)
                {
                    var scadeCmd = new SqlCommand("ScadeCantitatiIngredient", con, transaction)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    scadeCmd.Parameters.AddWithValue("@DishID", (int)row["DishID"]);
                    scadeCmd.Parameters.AddWithValue("@Multiplier", (int)row["Quantity"]);
                    scadeCmd.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Eroare la salvarea comenzii: " + ex.Message);
            }
        }

        public static List<UserOrder> GetUserOrders(int userId)
        {
            var orders = new List<UserOrder>();
            using var con = HelperDAL.Connection();
            con.Open();

            var cmd = new SqlCommand("GetUserOrders", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserID", userId);

            using var reader = cmd.ExecuteReader();

            int currentOrderId = -1;
            UserOrder currentOrder = null;

            while (reader.Read())
            {
                int orderId = reader.GetInt32(reader.GetOrdinal("OrderID"));
                if (orderId != currentOrderId)
                {
                    currentOrder = new UserOrder
                    {
                        OrderId = orderId,
                        UserId = userId,
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                        Items = new List<string>()
                    };
                    orders.Add(currentOrder);
                    currentOrderId = orderId;
                }

                string dishName = reader.GetString(reader.GetOrdinal("DishName"));
                int quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));

                currentOrder.Items.Add($"{dishName} x{quantity}");
            }

            return orders;
        }

        public static List<AdminOrder> GetAllOrders()
        {
            var orders = new List<AdminOrder>();
            using var con = HelperDAL.Connection();
            con.Open();

            var cmd = new SqlCommand("GetAllOrders", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            using var reader = cmd.ExecuteReader();

            int currentOrderId = -1;
            AdminOrder currentOrder = null;

            while (reader.Read())
            {
                int orderId = reader.GetInt32(reader.GetOrdinal("OrderID"));
                if (orderId != currentOrderId)
                {
                    currentOrder = new AdminOrder
                    {
                        OrderId = orderId,
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                        TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                        CustomerName = reader.GetString(reader.GetOrdinal("FullName")),
                        CustomerEmail = reader.GetString(reader.GetOrdinal("Email")),
                        Items = new List<string>()
                    };
                    orders.Add(currentOrder);
                    currentOrderId = orderId;
                }

                string dishName = reader.GetString(reader.GetOrdinal("DishName"));
                int quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));

                currentOrder.Items.Add($"{dishName} x{quantity}");
            }

            return orders;
        }
        public static void CancelOrder(int orderId)
        {
            using var con = HelperDAL.Connection();
            con.Open();

            var cmd = new SqlCommand("CancelOrder", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@OrderID", orderId);

            cmd.ExecuteNonQuery();
        }
        public static void UpdateOrderStatus(int orderId, string newStatus)
        {
            using var con = HelperDAL.Connection();
            con.Open();

            using var cmd = new SqlCommand("UpdateOrderStatus", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@OrderID", orderId);
            cmd.Parameters.AddWithValue("@NewStatus", newStatus);

            cmd.ExecuteNonQuery();
        }

    }
}
