using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterTest.Models;
using Implem.PleasanterTest.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.PleasanterTest.Operations
{
    internal class DeptsOperations
    {
        public static void Delete(string deptName)
        {
            var userModel = UserData.Get(UserData.UserTypes.TenantManager1);
            var id = Initializer.Depts.GetDeptIdByName(deptName);
            var context = ContextData.Get(
                userId: userModel.UserId,
                routeData: RouteData.DeptsDelete(id: id));
            DeptUtilities.Delete(
               context: context,
               ss: SiteSettingsUtilities.DeptsSiteSettings(context: context),
               deptId: context.Id.ToInt());
        }
    }
}
