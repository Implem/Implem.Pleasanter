using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Security
{
    public static class Passwords
    {
        public static string Change(int id)
        {
            return UserUtilities.ChangePassword(id);
        }

        public static string ChangeAtLogin()
        {
            return UserUtilities.ChangePasswordAtLogin();
        }

        public static string Reset(int userId)
        {
            return UserUtilities.ResetPassword(userId);
        }
    }
}