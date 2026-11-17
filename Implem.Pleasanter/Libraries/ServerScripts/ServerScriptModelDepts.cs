using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelDepts
    {
        private readonly Context Context;
        private HashSet<string> ReadableColumnNames;

        public ServerScriptModelDepts(Context context)
        {
            Context = context;
        }

        private HashSet<string> GetReadableColumnNames()
        {
            return ReadableColumnNames ??= ServerScriptUtilities.ReadableColumnNames(
                context: Context,
                ss: SiteSettingsUtilities.DeptsSiteSettings(context: Context));
        }

        public ServerScriptModelDeptModel Get(object id)
        {
            var dept = new Dept(
                context: Context,
                tenantId: Context.TenantId,
                deptId: id.ToInt());

            if (dept.Id <= 0)
            {
                return null;
            }
            var readableColumnNames = GetReadableColumnNames();
            var deptModel = new ServerScriptModelDeptModel(
                context: Context,
                tenantId: dept.TenantId,
                deptId: dept.Id,
                deptCode: dept.Code,
                deptName: dept.Name,
                body: dept.Body,
                disabled: dept.Disabled,
                extras: dept.Extras,
                readableColumnNames: readableColumnNames);
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