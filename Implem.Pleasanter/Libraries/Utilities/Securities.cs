using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
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
    }
}