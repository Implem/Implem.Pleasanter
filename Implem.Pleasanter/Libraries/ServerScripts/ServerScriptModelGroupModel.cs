using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
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

        public ServerScriptModelGroupModel(
            Context context,
            int tenantId,
            int groupId,
            string groupName,
            string body,
            bool disabled)
        {
            Context = context;
            TenantId = tenantId;
            GroupId = groupId;
            GroupName = groupName;
            Body = body;
            Disabled = disabled;
        }

        public List<ServerScriptModelGroupMemberModel> GetMembers()
        {
            var groupMembers = new List<ServerScriptModelGroupMemberModel>();
            GroupUtilities.GroupMembers(
                context: Context,
                groupId: GroupId)
                    .ForEach(dataRow =>
                        groupMembers.Add(new ServerScriptModelGroupMemberModel(
                            context: Context,
                            groupId: dataRow.Int("GroupId"),
                            deptId: dataRow.Int("DeptId"),
                            userId: dataRow.Int("UserId"),
                            admin: dataRow.Bool("Admin"))));
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
            var groupChildren = new List<ServerScriptModelGroupModel>();
            GroupUtilities.GroupChildren(
                context: Context,
                groupId: GroupId)
                    .ForEach(dataRow =>
                        groupChildren.Add(new ServerScriptModelGroups(context: Context)
                        .Get(id: dataRow.Int("GroupId"))));
            return groupChildren;
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