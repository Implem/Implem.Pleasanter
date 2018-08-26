using Implem.Pleasanter.Libraries.Requests;
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
        public static string SignIn(Context context, string returnUrl)
        {
            return new UserModel(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                setByForm: true)
                    .Authenticate(context: context, returnUrl: returnUrl);
        }

        public static bool Try(Context context, string loginId, string password)
        {
            return new UserModel(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                setByForm: true)
                    .Authenticate(context: context);
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