using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using System;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Ldap
    {
        public static bool Authenticate(Context context, string loginId, string password)
        {
            switch (Platform(context: context))
            {
                case "windows":
                    return LdapDs.Authenticate(
                        context: context,
                        loginId: loginId,
                        password: password);
                default:
                    return LdapNovelDs.Authenticate(
                        context: context,
                        loginId: loginId,
                        password: password);
            }
        }

        public static void UpdateOrInsert(Context context, string loginId)
        {
            switch (Platform(context: context))
            {
                case "windows":
                    LdapDs.UpdateOrInsert(
                        context: context,
                        loginId: loginId);
                    break;
                default:
                    LdapNovelDs.UpdateOrInsert(
                        context: context,
                        loginId: loginId);
                    break;
            }
        }

        public static void Sync(Context context)
        {
            switch (Platform(context: context))
            {
                case "windows":
                    LdapDs.Sync(context: context);
                    break;
                default:
                    LdapNovelDs.Sync(context: context);
                    break;
            }
        }

        private static string Platform(Context context)
        {
            if (Parameters.Authentication.DsProvider == "Novell")
            {
                return "others";
            }

            if (Environment.OSVersion?.ToString().ToLower().Contains("windows") == true) 
            {
                return "windows";
            }
            else
            {
                return "others";
            }
        }
    }
}