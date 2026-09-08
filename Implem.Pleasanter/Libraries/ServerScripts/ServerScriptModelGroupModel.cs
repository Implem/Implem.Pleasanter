using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelGroupModel
    {
        private readonly Context Context;
        private readonly int TenantId;
        public readonly int GroupId;
        public readonly string GroupName;
        public readonly string Body;
        public readonly bool Disabled;
        private readonly Dictionary<string, object> Extras;

        public ServerScriptModelGroupModel(
            Context context,
            int tenantId,
            int groupId,
            string groupName,
            string body,
            bool disabled,
            Dictionary<string, object> extras)
        {
            Context = context;
            TenantId = tenantId;
            GroupId = groupId;
            GroupName = groupName;
            Body = body;
            Disabled = disabled;
            Extras = extras;
        }

        public object ToJson()
        {
            dynamic d = new ExpandoObject();
            var dict = (IDictionary<string, object>)d;
            dict["TenantId"] = TenantId;
            dict["GroupId"] = GroupId;
            dict["GroupName"] = GroupName;
            dict["Body"] = Body;
            dict["Disabled"] = Disabled;
            ServerScriptUtilities.MergeExtras(dict, Extras);
            return d;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(ToJson());
        }

        public List<ServerScriptModelGroupMemberModel> GetMembers()
        {
            var detail = GroupUtilities.GroupMembersDetail(
                context: Context,
                groupId: GroupId).ToList();
            var userIds = detail
                .Select(dataRow => dataRow.Int("UserId"))
                .Where(id => id > 0)
                .Distinct()
                .ToList();
            var deptIds = detail
                .Select(dataRow => dataRow.Int("DeptId"))
                .Where(id => id > 0)
                .Distinct()
                .ToList();
            var childGroupIds = detail
                .Select(dataRow => dataRow.Int("GroupId"))
                .Where(id => id > 0)
                .Distinct()
                .ToList();
            var userExtrasHash = userIds.Any()
                ? Repository.ExecuteTable(
                    context: Context,
                    statements: Rds.SelectUsers(
                        column: DataTypes.User.QueryColumnWithExtras(),
                        where: Rds.UsersWhere()
                            .TenantId(Context.TenantId)
                            .UserId_In(userIds)))
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow.Int("UserId"),
                            dataRow => ServerScriptUtilities.BuildExtras(dataRow, "Users"))
                : new Dictionary<int, Dictionary<string, object>>();
            var deptExtrasHash = deptIds.Any()
                ? Repository.ExecuteTable(
                    context: Context,
                    statements: Rds.SelectDepts(
                        column: DataTypes.Dept.QueryColumnWithExtras(),
                        where: Rds.DeptsWhere()
                            .TenantId(Context.TenantId)
                            .DeptId_In(deptIds)))
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow.Int("DeptId"),
                            dataRow => ServerScriptUtilities.BuildExtras(dataRow, "Depts"))
                : new Dictionary<int, Dictionary<string, object>>();
            var groupExtrasHash = childGroupIds.Any()
                ? Repository.ExecuteTable(
                    context: Context,
                    statements: Rds.SelectGroups(
                        column: DataTypes.Group.QueryColumnWithExtras(),
                        where: Rds.GroupsWhere()
                            .TenantId(Context.TenantId)
                            .GroupId_In(childGroupIds)))
                        .AsEnumerable()
                        .ToDictionary(
                            dataRow => dataRow.Int("GroupId"),
                            dataRow => ServerScriptUtilities.BuildExtras(dataRow, "Groups"))
                : new Dictionary<int, Dictionary<string, object>>();
            var groupMembers = new List<ServerScriptModelGroupMemberModel>();
            detail.ForEach(dataRow =>
            {
                var userId = dataRow.Int("UserId");
                var deptId = dataRow.Int("DeptId");
                var childGroupId = dataRow.Int("GroupId");
                Dictionary<string, object> extras;
                if (userId > 0)
                {
                    extras = userExtrasHash.Get(userId);
                }
                else if (deptId > 0)
                {
                    extras = deptExtrasHash.Get(deptId);
                }
                else if (childGroupId > 0)
                {
                    extras = groupExtrasHash.Get(childGroupId);
                }
                else
                {
                    extras = null;
                }
                groupMembers.Add(new ServerScriptModelGroupMemberModel(
                    context: Context,
                    groupId: childGroupId,
                    groupName: dataRow.String("GroupName"),
                    deptId: deptId,
                    deptName: dataRow.String("DeptName"),
                    deptCode: dataRow.String("DeptCode"),
                    userId: userId,
                    loginId: dataRow.String("LoginId"),
                    name: dataRow.String("Name"),
                    userCode: dataRow.String("UserCode"),
                    tenantManager: dataRow.Bool("TenantManager"),
                    disabled: dataRow.Bool("Disabled"),
                    admin: dataRow.Bool("Admin"),
                    extras: extras));
            });
            return groupMembers;
        }

        public bool ContainsDept(int deptId)
        {
            return GroupUtilities.Contains(
                context: Context,
                groupId: GroupId,
                dept: SiteInfo.Dept(
                    tenantId: Context.TenantId,
                    deptId: deptId));
        }

        public bool ContainsDept(ServerScriptModelDeptModel dept)
        {
            return GroupUtilities.Contains(
                context: Context,
                groupId: GroupId,
                dept: SiteInfo.Dept(
                    tenantId: Context.TenantId,
                    deptId: dept?.DeptId ?? 0));
        }

        public bool ContainsUser(int userId)
        {
            return GroupUtilities.Contains(
                context: Context,
                groupId: GroupId,
                user: SiteInfo.User(
                    context: Context,
                    userId: userId.ToInt()));
        }

        public bool ContainsUser(ServerScriptModelUserModel user)
        {
            return GroupUtilities.Contains(
                context: Context,
                groupId: GroupId,
                user: SiteInfo.User(
                    context: Context,
                    userId: user?.UserId ?? 0));
        }

        public List<ServerScriptModelGroupModel> GetChildren()
        {
            var childIds = GroupUtilities.GroupChildren(
                context: Context,
                groupId: GroupId)
                    .Select(dataRow => dataRow.Int("GroupId"))
                    .ToList();
            if (!childIds.Any())
            {
                return new List<ServerScriptModelGroupModel>();
            }
            var dataTable = Repository.ExecuteTable(
                context: Context,
                statements: Rds.SelectGroups(
                    column: DataTypes.Group.QueryColumnWithExtras(),
                    where: Rds.GroupsWhere()
                        .TenantId(Context.TenantId)
                        .GroupId_In(childIds),
                    orderBy: Rds.GroupsOrderBy().GroupId()));
            return dataTable.AsEnumerable()
                .Select(dataRow => new DataTypes.Group(dataRow: dataRow))
                .Select(g => new ServerScriptModelGroupModel(
                    context: Context,
                    tenantId: g.TenantId,
                    groupId: g.Id,
                    groupName: g.Name,
                    body: g.Body,
                    disabled: g.Disabled,
                    extras: g.Extras))
                .ToList();
        }

        public bool ContainsChild(int childId)
        {
            return GroupUtilities.Contains(
                context: Context,
                groupId: GroupId,
                child: SiteInfo.Group(
                    tenantId: Context.TenantId,
                    groupId: childId));
        }

        public bool ContainsChild(ServerScriptModelGroupModel group)
        {
            return GroupUtilities.Contains(
                context: Context,
                groupId: GroupId,
                child: SiteInfo.Group(
                    tenantId: Context.TenantId,
                    groupId: group?.GroupId ?? 0));
        }
    }
}