﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Configuration;
using System.IdentityModel.Services;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
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
            FormsAuthentication.SignOut();
            FederatedAuthentication.SessionAuthenticationModule?.DeleteSessionTokenCookie();
            SessionUtilities.Abandon(context: context);
        }

        public static bool Windows()
        {
            return ((AuthenticationSection)ConfigurationManager
                .GetSection("system.web/authentication")).Mode.ToString() == "Windows";
        }

        public static bool SSO(Context context)
        {
            return Windows() || SAML();
        }

        public static bool SAML()
        {
            return Parameters.Authentication.Provider == "SAML"
                || Parameters.Authentication.Provider == "SAML-MultiTenant";
        }

        public static string Token()
        {
            return HttpContext.Current?.Request?.Cookies["ASP.NET_SessionId"]?.Value.Sha512Cng();
        }
    }
}