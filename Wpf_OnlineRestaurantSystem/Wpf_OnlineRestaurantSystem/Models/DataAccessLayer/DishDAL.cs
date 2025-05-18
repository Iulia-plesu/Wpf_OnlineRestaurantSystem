using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using Wpf_OnlineRestaurantSystem.ViewModels;

namespace Wpf_OnlineRestaurantSystem.Models
{
    public static class DishDAL
    {
        public static List<Allergen> GetAllAllergens()
        {
            var allergens = new List<Allergen>();
            using (var con = HelperDAL.Connection())
            {
                con.Open();
                var cmd = new SqlCommand("GetAllAllergens", con);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allergens.Add(new Allergen
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return allergens;
        }

        public static List<Allergen> GetDishAllergens(int dishId)
        {
            var allergens = new List<Allergen>();
            using (var con = HelperDAL.Connection())
            {
                con.Open();
                var cmd = new SqlCommand("GetDishAllergens", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DishID", dishId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        allergens.Add(new Allergen
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return allergens;
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

        public static List<Dish> GetAllDishes()
        {
            var dishes = new List<Dish>();
            var dishAllergens = new Dictionary<int, List<string>>();

            using (var con = HelperDAL.Connection())
            {
                con.Open();
                var cmd = new SqlCommand("GetAllDishesWithDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = cmd.ExecuteReader())
                {
                    // First result set: dishes
                    while (reader.Read())
                    {
                        dishes.Add(new Dish
                        {
                            Id = reader.GetInt32(0),                            // DishID
                            Name = reader.GetString(1),                         // Name
                            QuantityPerPortion = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Price = reader.GetDecimal(4),
                            Available = reader.GetBoolean(5),
                            TotalQuantity = reader.IsDBNull(6) ? null : reader.GetString(6),
                            CategoryId = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                            IsPartOfMenu = reader.GetBoolean(8),
                            Unit = "" // Default empty string
                        });
                    }

                    // Second result set: dish allergens
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            int dishId = reader.GetInt32(0);
                            string allergenName = reader.GetString(2);

                            if (!dishAllergens.ContainsKey(dishId))
                            {
                                dishAllergens[dishId] = new List<string>();
                            }
                            dishAllergens[dishId].Add(allergenName);
                        }
                    }
                }
            }

            // Combine dishes with their allergens
            foreach (var dish in dishes)
            {
                if (dishAllergens.TryGetValue(dish.Id, out var allergens))
                {
                    dish.Allergens = string.Join(", ", allergens);
                }
                else
                {
                    dish.Allergens = "";
                }
            }

            return dishes;
        }

        public static void CreateDish(Dish dish, List<int> allergenIds)
        {
            using (var con = HelperDAL.Connection())
            {
                con.Open();
                var cmd = new SqlCommand("CreateDish", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Name", dish.Name);
                cmd.Parameters.AddWithValue("@Price", dish.Price);
                cmd.Parameters.AddWithValue("@IsPartOfMenu", dish.IsPartOfMenu);
                cmd.Parameters.AddWithValue("@QuantityPerPortion", (object)dish.QuantityPerPortion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object)dish.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Available", dish.Available);
                cmd.Parameters.AddWithValue("@TotalQuantity", (object)dish.TotalQuantity ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CategoryId", (object)dish.CategoryId ?? DBNull.Value);

                string allergenIdsString = allergenIds != null && allergenIds.Count > 0
                    ? string.Join(",", allergenIds)
                    : null;

                cmd.Parameters.AddWithValue("@AllergenIDs", (object)allergenIdsString ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateDish(Dish dish, List<int> allergenIds)
        {
            using (var con = HelperDAL.Connection())
            {
                con.Open();
                var cmd = new SqlCommand("UpdateDish", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DishID", dish.Id);
                cmd.Parameters.AddWithValue("@Name", dish.Name);
                cmd.Parameters.AddWithValue("@Price", dish.Price);
                cmd.Parameters.AddWithValue("@IsPartOfMenu", dish.IsPartOfMenu);
                cmd.Parameters.AddWithValue("@QuantityPerPortion", (object)dish.QuantityPerPortion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object)dish.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Available", dish.Available);
                cmd.Parameters.AddWithValue("@TotalQuantity", (object)dish.TotalQuantity ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CategoryId", (object)dish.CategoryId ?? DBNull.Value);

                string allergenIdsString = allergenIds != null && allergenIds.Count > 0
                    ? string.Join(",", allergenIds)
                    : null;

                cmd.Parameters.AddWithValue("@AllergenIDs", (object)allergenIdsString ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteDish(int dishId)
        {
            using (var con = HelperDAL.Connection())
            {
                con.Open();
                var cmd = new SqlCommand("DeleteDish", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DishID", dishId);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public class Allergen
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}