using System.Configuration;
using Microsoft.Data.SqlClient;

namespace Wpf_OnlineRestaurantSystem.Models
{
    class HelperDAL
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["myConStr"].ConnectionString;

        internal static SqlConnection Connection()
        {
           return new SqlConnection(connectionString);
            
        }
    }
}
