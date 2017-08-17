using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
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

        public static bool Try(string loginId, string password)
        {
            return new UserModel(SiteSettingsUtilities.UsersSiteSettings(), setByForm: true)
                .Authenticate();
        }

        public static void SignOut()
        {
            FormsAuthentication.SignOut();
            HttpContext.Current.Session.Abandon();
        }

        public static bool Windows()
        {
            return ((AuthenticationSection)ConfigurationManager
                .GetSection("system.web/authentication")).Mode.ToString() == "Windows";
        }
    }
}