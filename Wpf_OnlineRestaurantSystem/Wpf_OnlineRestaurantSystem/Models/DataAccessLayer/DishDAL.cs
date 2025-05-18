using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using Wpf_OnlineRestaurantSystem.ViewModels;

namespace Wpf_OnlineRestaurantSystem.Models
{
    public static class DishDAL
    {
        public static List<Dish> GetAllDishes()
        {
            var dishes = new List<Dish>();
            using (var con = HelperDAL.Connection())
            {
                con.Open();
                var cmd = new SqlCommand("GetAllDishesWithStock", con);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dishes.Add(new Dish
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            CurrentQuantity = reader.IsDBNull(2) ? "0" : reader.GetString(2)
                        });
                    }
                }
            }
            return dishes;
        }

        public static void UpdateDishStock(int dishId, int quantityToAdd)
        {
            using (var con = HelperDAL.Connection())
            {
                con.Open();
                var cmd = new SqlCommand("UpdateDishStock", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DishID", dishId);
                cmd.Parameters.AddWithValue("@QuantityToAdd", quantityToAdd);
                cmd.ExecuteNonQuery();
            }
        }
    }
}