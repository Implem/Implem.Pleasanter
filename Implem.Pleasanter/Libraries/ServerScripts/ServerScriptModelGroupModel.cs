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

        public ServerScriptModelGroupModel(
            Context context,
            int tenantId,
            int groupId,
            string groupName)
        {
            Context = context;
            TenantId = tenantId;
            GroupId = groupId;
            GroupName = groupName;
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
    }
}