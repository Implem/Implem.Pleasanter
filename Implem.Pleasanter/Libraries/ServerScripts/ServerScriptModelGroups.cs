using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;

namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelGroups
    {
        private readonly Context Context;
        private HashSet<string> ReadableColumnNames;

        public ServerScriptModelGroups(Context context)
        {
            Context = context;
        }

        private HashSet<string> GetReadableColumnNames()
        {
            return ReadableColumnNames ??= ServerScriptUtilities.ReadableColumnNames(
                context: Context,
                ss: SiteSettingsUtilities.GroupsSiteSettings(context: Context));
        }

        public ServerScriptModelGroupModel Get(object id)
        {
            var group = new Group(
                context: Context,
                tenantId: Context.TenantId,
                groupId: id.ToInt());
            if (group.Id <= 0)
            {
                return null;
            }
            var readableColumnNames = GetReadableColumnNames();
            var groupModel = new ServerScriptModelGroupModel(
                context: Context,
                tenantId: group.TenantId,
                groupId: group.Id,
                groupName: group.Name,
                body: group.Body,
                disabled: group.Disabled,
                extras: group.Extras,
                readableColumnNames: readableColumnNames);
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