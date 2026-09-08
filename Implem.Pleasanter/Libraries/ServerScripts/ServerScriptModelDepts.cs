using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelDepts
    {
        private readonly Context Context;

        public ServerScriptModelDepts(Context context)
        {
            Context = context;
        }

        public ServerScriptModelDeptModel Get(object id)
        {
            var dept = new Dept(
                context: Context,
                tenantId: Context.TenantId,
                deptId: id.ToInt());

            var deptModel = dept.Id > 0
                ? new ServerScriptModelDeptModel(
                    context: Context,
                    tenantId: dept.TenantId,
                    deptId: dept.Id,
                    deptCode: dept.Code,
                    deptName: dept.Name,
                    body: dept.Body,
                    disabled: dept.Disabled,
                    extras: dept.Extras)
                : null;
            return deptModel;
        }

        public bool Update(object id, string model)
        {
            var apiContext = ServerScriptUtilities.CreateContext(
                context: Context,
                controller: "Depts",
                action: "Update",
                id: id.ToInt(),
                apiRequestBody: model);
            return DeptUtilities.UpdateByServerScript(
                context: apiContext,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: apiContext),
                deptId: id.ToInt());
        }

        public bool Create(string model)
        {
            var id = 0;
            var apiContext = ServerScriptUtilities.CreateContext(
                context: Context,
                controller: "Depts",
                action: "Create",
                id: id,
                apiRequestBody: model);
            return DeptUtilities.CreateByServerScript(
                context: apiContext,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: apiContext),
                deptId: id);
        }
    }
}