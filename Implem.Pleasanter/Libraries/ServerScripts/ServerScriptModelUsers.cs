using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelUsers
    {
        private readonly Context Context;

        public ServerScriptModelUsers(Context context)
        {
            Context = context;
        }

        public ServerScriptModelUserModel Get(object id)
        {
            var user = SiteInfo.User(
                context: Context,
                userId: id.ToInt());
            var userModel = !user.Anonymous()
                ? new ServerScriptModelUserModel(
                    context: Context,
                    tenantId: user.TenantId,
                    userId: user.Id,
                    deptId: user.DeptId,
                    loginId: user.LoginId,
                    name: user.Name,
                    userCode: user.UserCode,
                    tenantManager: user.TenantManager,
                    serviceManager: user.ServiceManager,
                    disabled: user.Disabled)
                : null;
            return userModel;
        }
    }
}