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
    public static class GroupChildUtilities
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static int[] GetParentIds(
            Context context,
            int startId)
        {
            return GetParentIds(context: context, startIds: new int[] { startId });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static int[] GetParentIds(
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
        public static int[] GetChildrenIds(
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
            var ids = new List<int>();
            ids.AddRange(GetChildrenIds(context: context, startIds: startChldIds));
            ids.AddRange(GetParentIds(context: context, startId: groupId));
            ids.AddRange(startChldIds);
            var graph = new Dictionary<int, int[]>();
            Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectGroupChildren(
                    column: Rds.GroupChildrenColumn().GroupId().ChildId(),
                    where: Rds.GroupChildrenWhere().GroupId_In(value: ids)))
                    .AsEnumerable()
                    .Select(dataSet => (groupId: dataSet.Int("GroupId"), childId: dataSet.Int("ChildId")))
                    .GroupBy(kv => kv.groupId)
                    .ForEach(kv => graph[kv.Key] = kv.Select(v => v.childId).ToArray());
            graph[groupId] = startChldIds.ToArray();
            var checkCycle = CheckGroupChildCycle(graph: graph, Parameters.General.GroupsDepthMax);
            if (checkCycle.status == Error.Types.CircularGroupChild)
            {
                new SysLogModel(
                    context: context,
                    method: context.HttpMethod,
                        message: $"Failed to update Group. Circular reference error.(GroupId={checkCycle.groupIdx})",
                        sysLogType: SysLogModel.SysLogTypes.UserError);
            }
            return checkCycle.status;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public static (Error.Types status, int groupIdx) CheckGroupChildCycle(Dictionary<int, int[]> graph, int lvMax)
        {
            foreach (var key in graph.Keys)
            {
                var parentIds = new Stack<int>();
                var ret = Scanning(graph, key, parentIds, 1, lvMax);
                if (ret.status != Error.Types.None) return ret;
            }
            return (status: Error.Types.None, groupIdx: 0);
            (Error.Types status, int groupIdx) Scanning(Dictionary<int, int[]> graph, int idx, Stack<int> parentIds, int lv, int lvMax)
            {
                if (lv > lvMax) return (status: Error.Types.GroupDepthMax, groupIdx: idx);
                if (!graph.ContainsKey(idx)) return (status: Error.Types.None, groupIdx: 0);
                parentIds.Push(idx);
                foreach (var idx1 in graph[idx])
                {
                    if (parentIds.Contains(idx1)) return (status: Error.Types.CircularGroupChild, groupIdx: idx);
                    var val = Scanning(graph, idx1, parentIds, lv + 1, lvMax);
                    if (val.status != Error.Types.None) return val;
                }
                parentIds.Pop();
                return (status: Error.Types.None, groupIdx: 0);
            }
        }
    }
}
