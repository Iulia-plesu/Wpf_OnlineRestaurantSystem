using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Wpf_OnlineRestaurantSystem.Models;

namespace Wpf_OnlineRestaurantSystem.Models
{
    public static class UserDAL
    {
        public static User GetUserByEmailAndPassword(string email, string password)
        {
            try
            {
                using (SqlConnection con = HelperDAL.Connection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT * FROM Users WHERE Email = @Email AND PasswordHash = @PasswordHash", con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PasswordHash", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserID")),
                                    FirstName = reader.GetString(reader.GetOrdinal("Username")),
                                    LastName = reader.GetString(reader.GetOrdinal("FullName")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    Password = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Eroare în UserDAL: " + ex.Message);
            }

            return null;
        }
        public static bool RegisterUser(User newUser)
        {
            try
            {
                using (SqlConnection con = HelperDAL.Connection())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO Users 
                        (Username, FullName, Email, PhoneNumber, Address, PasswordHash, Role, CreatedAt)
                        VALUES 
                        (@Username, @FullName, @Email, @PhoneNumber, @Address, @PasswordHash, @Role, GETDATE())", con))
                    {
                        cmd.Parameters.AddWithValue("@Username", newUser.FirstName);
                        cmd.Parameters.AddWithValue("@FullName", newUser.LastName); 
                        cmd.Parameters.AddWithValue("@Email", newUser.Email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", newUser.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Address", newUser.Address);
                        cmd.Parameters.AddWithValue("@PasswordHash", newUser.Password);
                        cmd.Parameters.AddWithValue("@Role", "User"); 

                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Eroare la înregistrare: " + ex.Message);
                return false;
            }
        }
        public static List<User> GetNormalUsers()
        {
            var users = new List<User>();

            using (var con = HelperDAL.Connection())
            {
                con.Open();

                using (var cmd = new SqlCommand(@"
            SELECT * FROM Users 
            WHERE Email NOT LIKE '%@admin%' AND Email NOT LIKE '%@employee%'", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                Id = (int)reader["UserID"],
                                FirstName = reader["Username"].ToString(),
                                LastName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Address = reader["Address"].ToString(),
                                Password = reader["PasswordHash"].ToString()
                            });
                        }
                    }
                }
            }

            return users;
        }


    }
}
