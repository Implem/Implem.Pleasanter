using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.PleasanterTest.Operations
{
    public class TenantOperations
    {
        public static void Update(UserData.UserTypes userType, Forms forms)
        {
            var userModel = UserData.Get(userType: userType);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.TenantsUpdate(),
                forms: forms);
            TenantUtilities.Update(
                context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                    tenantId: context.TenantId);
        }
    }
}
