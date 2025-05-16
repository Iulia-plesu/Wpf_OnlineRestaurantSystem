using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Wpf_OnlineRestaurantSystem.Models
{
    public static class CategoryDAL
    {
        public static List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            using (SqlConnection con = HelperDAL.Connection())
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("GetAllCategories", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }

            return categories;
        }
    }
}
