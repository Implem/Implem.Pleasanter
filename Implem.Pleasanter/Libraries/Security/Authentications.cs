using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web.Security;
namespace Implem.Pleasanter.Libraries.Security
{
    public static class Authentications
    {
        public static string SignIn(string returnUrl)
        {
            return new UserModel(
                SiteSettingsUtilities.UsersSiteSettings(),
                setByForm: true)
                    .Authenticate(returnUrl);
        }

        public static void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}