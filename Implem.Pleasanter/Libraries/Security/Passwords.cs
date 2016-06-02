using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Security
{
    public static class Passwords
    {
        public static string Change(int id)
        {
            return new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Types.NotSet,
                id,
                setByForm: true)
                    .ChangePassword();
        }

        public static string ChangeAtLogin()
        {
            return new UserModel(Forms.Data("Users_LoginId")).ChangePasswordAtLogin();
        }

        public static string Reset(int id)
        {
            return new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Types.NotSet,
                id,
                setByForm: true)
                    .ResetPassword();
        }

        public static string Default()
        {
            return "pleasanter";
        }
    }
}