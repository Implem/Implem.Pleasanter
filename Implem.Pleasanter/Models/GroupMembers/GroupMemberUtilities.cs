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
                    oldParents: GroupChildUtilities.GetParentIds(context: context, startId: groupId)));
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
                oldParents: GroupChildUtilities.GetParentIds(context: context, startId: groupId));
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
    }
}
