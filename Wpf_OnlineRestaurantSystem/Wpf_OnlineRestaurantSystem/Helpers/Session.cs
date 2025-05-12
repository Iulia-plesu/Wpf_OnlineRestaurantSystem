using Wpf_OnlineRestaurantSystem.Models;

namespace Wpf_OnlineRestaurantSystem.Helpers
{
    public static class Session
    {
        public static User CurrentUser { get; set; }
        public static int GetCurrentUserId()
        {
            return CurrentUser?.Id ?? -1;
        }
        public static string GetCurrentUserEmail()
        {
            return CurrentUser?.Email ?? string.Empty;
        }
        public static string GetCurrentUserRole()
        {
            return CurrentUser?.Role ?? "Customer";
        }

    }
}
