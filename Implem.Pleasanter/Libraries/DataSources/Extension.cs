using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System;
namespace Implem.Pleasanter.Libraries.DataSources
{
    public static class Extension
    {
        public static bool Authenticate(
            Context context,
            string loginId,
            string password,
            UserModel userModel)
        {
            throw new NotImplementedException();
        }

        public static void SwichTenant(Context context)
        {
        }

        public static void SetSwichTenant(Context context, int targetTenantId)
        {
        }

        public static void UnsetSwichTenant(Context context)
        {
        }
    }
}