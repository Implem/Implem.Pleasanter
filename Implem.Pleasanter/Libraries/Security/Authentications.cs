using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Security
{
    public static class Authentications
    {
        public enum AuthenticationCodeCharacterTypes
        {
            Number,
            Letter,
            NumberAndLetter
        }

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
            SignOutLog(context: context);
            context.FormsAuthenticationSignOut();
            context.FederatedAuthenticationSessionAuthenticationModuleDeleteSessionTokenCookie();
            SessionUtilities.Abandon(context: context);
        }

        private static void SignOutLog(Context context)
        {
            if (Parameters.SysLog.SignOut)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(SignOut),
                    message: new
                    {
                        LoginId = context.LoginId
                    }.ToJson());
            }
        }

        public static bool Windows(Context context)
        {
            return context.AuthenticationsWindows();
        }

        public static bool DisableDeletingSiteAuthentication(Context context)
        {
            return Parameters.Security.DisableDeletingSiteAuthentication == true
                || Windows(context: context) || SAML();
        }

        public static bool SAML()
        {
            return Parameters.Authentication.Provider == "SAML"
                || Parameters.Authentication.Provider == "SAML-MultiTenant";
        }

    }
}