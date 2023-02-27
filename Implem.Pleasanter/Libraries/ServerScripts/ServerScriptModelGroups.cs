using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;

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
                    groupName: group.Name,
                    body: group.Body,
                    disabled: group.Disabled)
                : null;
            return groupModel;
        }

        public bool Update(object id, string model)
        {
            var apiContext = ServerScriptUtilities.CreateContext(
                context: Context,
                controller: "Groups",
                action: "Update",
                id: id.ToInt(),
                apiRequestBody: model);
            return GroupUtilities.UpdateByServerScript(
                context: apiContext,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: apiContext),
                groupId: id.ToInt(),
                model: model);
        }
    }
}