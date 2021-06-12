using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelGroups
    {
        private readonly Context Context;

        public ServerScriptModelGroups(Context context)
        {
            Context = context;
        }

        public ServerScriptModelGroupModel Get(object id)
        {
            var group = SiteInfo.Group(
                tenantId: Context.TenantId,
                groupId: id.ToInt());
            var groupModel = group.Id > 0
                ? new ServerScriptModelGroupModel(
                    context: Context,
                    tenantId: group.TenantId,
                    groupId: group.Id,
                    groupName: group.Name)
                : null;
            return groupModel;
        }
    }
}