using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
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
            var dept = SiteInfo.Dept(
                tenantId: Context.TenantId,
                deptId: id.ToInt());
            var deptModel = dept.Id > 0
                ? new ServerScriptModelDeptModel(
                    context: Context,
                    tenantId: dept.TenantId,
                    deptId: dept.Id,
                    deptCode: dept.Code,
                    deptName: dept.Name)
                : null;
            return deptModel;
        }
    }
}