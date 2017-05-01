using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web;
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

        public static bool Try(string password)
        {
            return new UserModel(SiteSettingsUtilities.UsersSiteSettings(), Sessions.UserId())
            {
                Password = password
            }.Authenticate();
        }

        public static void SignOut()
        {
            FormsAuthentication.SignOut();
            HttpContext.Current.Session.Abandon();
        }
    }
}