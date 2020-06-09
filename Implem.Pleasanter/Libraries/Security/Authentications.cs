using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Security
{
    public static class Authentications
    {
        public static string SignIn(Context context, string returnUrl)
        {
            return new UserModel(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                formData: context.Forms)
                    .Authenticate(
                        context: context,
                        returnUrl: returnUrl);
        }

        public static bool Try(Context context, string loginId, string password)
        {
            return new UserModel(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                formData: context.Forms)
                    .Authenticate(context: context);
        }

        public static void SignOut(Context context)
        {
            context.FormsAuthenticationSignOut();
            context.FederatedAuthenticationSessionAuthenticationModuleDeleteSessionTokenCookie();
            SessionUtilities.Abandon(context: context);

        }

        public static bool Windows(Context context)
        {
            return context.AuthenticationsWindows();
        }

        public static bool SSO(Context context)
        {
            return Windows(context: context) || SAML();
        }

        public static bool SAML()
        {
            return Parameters.Authentication.Provider == "SAML"
                || Parameters.Authentication.Provider == "SAML-MultiTenant";
        }
    }
}