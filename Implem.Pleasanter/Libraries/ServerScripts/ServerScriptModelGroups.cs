using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
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
            var group = new Group(
                context: Context,
                tenantId: Context.TenantId,
                groupId: id.ToInt());
            var groupModel = group.Id > 0
                ? new ServerScriptModelGroupModel(
                    context: Context,
                    tenantId: group.TenantId,
                    groupId: group.Id,
                    groupName: group.Name,
                    body: group.Body,
                    disabled: group.Disabled,
                    extras: group.Extras)
                : null;
            return groupModel;
        }

        public bool Create(string model)
        {
            var id = 0;
            var apiContext = ServerScriptUtilities.CreateContext(
                context: Context,
                controller: "Groups",
                action: "Create",
                id: id,
                apiRequestBody: model);
            return GroupUtilities.CreateByServerScript(
                context: apiContext,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: apiContext),
                groupId: id);
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
                groupId: id.ToInt());
        }
    }
}