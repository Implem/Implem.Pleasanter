using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
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
            var userModel = new UserModel(Forms.Data("Users_LoginId"));
            var error = userModel.ChangePasswordAtLogin();
            return error.Has()
                ? error.MessageJson()
                : userModel.Allow(Forms.Data("ReturnUrl"), atLogin: true);
        }

        public static string Reset(int userId)
        {
            return UserUtilities.ResetPassword(userId);
        }

        public static string Default()
        {
            return "pleasanter";
        }
    }
}