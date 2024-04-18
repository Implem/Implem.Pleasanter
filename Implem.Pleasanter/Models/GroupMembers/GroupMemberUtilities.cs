using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    public static class GroupMemberUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static void SyncGroupMembers(
            Context context,
            int groupId)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: SyncGroupMembersSql(
                    tenantId: context.TenantId,
                    groupId: groupId,
                    oldParents: GetParentIds(context: context, startId: groupId)));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlStatement DeleteGroupMembers(
            Context context,
            int groupId)
        {
            // groupIdの子ユーザを削除（親階層を含む）
            return SyncGroupMembersSql(
                tenantId: context.TenantId,
                groupId: groupId,
                oldParents: GetParentIds(context: context, startId: groupId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlStatement BulkDelete(
            Context context,
            IEnumerable<int> selected)
        {
            // selectedのIDの全ての子ユーザを削除
            return SyncGroupMembersSql(
                tenantId: context.TenantId,
                groupId: 0,
                oldParents: GetParentIds(context: context, startIds: selected));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static SqlStatement RefreshAllChildMembers(
            int tenantId)
        {
            // TenantIdに含まれる全ての子ユーザを再作成
            return SyncGroupMembersSql(
                tenantId: tenantId,
                groupId: 0,
                oldParents: new int[] { });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static int[] GetParentIds(
            Context context,
            int startId)
        {
            return GetParentIds(context: context, startIds: new int[] { startId });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static int[] GetParentIds(
            Context context,
            IEnumerable<int> startIds)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: new SqlStatement(
                    commandText: Def.Sql.GetGroupParentIds.Replace("{{GroupsStartIdP}}", string.Join(",", startIds)),
                    param: new SqlParamCollection {
                        { "GroupsDepthMax", Implem.DefinitionAccessor.Parameters.General.GroupsDepthMax },
                    }))
                    .AsEnumerable()
                    .Select(v => v.Int("GroupId"))
                    .ToArray();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static int[] GetChildrenIds(
            Context context,
            IEnumerable<int> startIds)
        {
            return Repository.ExecuteTable(
                context: context,
                statements: new SqlStatement(
                    commandText: Def.Sql.GetGroupChildrenIds.Replace("{{GroupsStartIdP}}", string.Join(",", startIds)),
                    param: new SqlParamCollection {
                        { "GroupsDepthMax", Implem.DefinitionAccessor.Parameters.General.GroupsDepthMax },
                    }))
                    .AsEnumerable()
                    .Select(v => v.Int("GroupId"))
                    .ToArray();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static SqlStatement SyncGroupMembersSql(
            int tenantId,
            int groupId,
            IEnumerable<int> oldParents)
        {
            var list = new List<int>(oldParents);
            if (groupId != 0) list.Add(groupId);
            var groupidSearchCondition = list.Any()
                ? "\"GroupId\" in ({0})".Params(string.Join(",", list))
                : "1=1";
            return new SqlStatement(
                    commandText: Def.Sql.RefreshAllChildMembers
                        .Replace("{{groupid_search_condition}}", groupidSearchCondition),
                    param: new SqlParamCollection {
                        { "TenantId", tenantId },
                        { "GroupsDepthMax", Implem.DefinitionAccessor.Parameters.General.GroupsDepthMax },
                    });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types CheckCircularGroup(
            Context context,
            int groupId,
            bool disabled,
            IEnumerable<string> children)
        {
            if (disabled) return Error.Types.None;
            if (children == null || !children.Any()) return Error.Types.None;
            var startChldIds = children?.Select(o =>
            (
                o.StartsWith("Group,")
                    ? o.Split_2nd().ToInt()
                    : 0
            ));
            var childrenIds = GetChildrenIds(context: context, startIds: startChldIds);
            if (!childrenIds.Any()) return Error.Types.None;
            if (childrenIds.Contains(groupId)) return Error.Types.CircularGroupChild;
            var parentids = GetParentIds(context: context, startId: groupId);
            return parentids.Any(v => childrenIds.Contains(v))
                ? Error.Types.CircularGroupChild
                : Error.Types.None;
        }
    }
}
