using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web.Security;
namespace Implem.Pleasanter.Libraries.Utilities
{
    public static class Securities
    {
        public static string Authenticate(string returnUrl)
        {
            return new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Types.NotSet,
                setByForm: true)
                    .Authenticate(returnUrl);
        }

        public static void Logout()
        {
            FormsAuthentication.SignOut();
        }

        public static string ChangePassword(int id)
        {
            return new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Types.NotSet,
                id,
                setByForm: true)
                    .ChangePassword();
        }

        public static string ChangePasswordAtLogin()
        {
            return new UserModel(Forms.Data("Users_LoginId")).ChangePasswordAtLogin();
        }

        public static string ResetPassword(int id)
        {
            return new UserModel(
                SiteSettingsUtility.UsersSiteSettings(),
                Permissions.Types.NotSet,
                id,
                setByForm: true)
                    .ResetPassword();
        }

        public static string DefaultAdminPassword()
        {
            return "pleasanter";
        }
    }
}