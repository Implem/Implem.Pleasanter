using Implem.Pleasanter.Libraries.Requests;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelGroupMemberModel
    {
        private readonly Context Context;
        public readonly int GroupId;
        public readonly string GroupName;
        public readonly int DeptId;
        public readonly string DeptName;
        public readonly string DeptCode;
        public readonly int UserId;
        public readonly string LoginId;
        public readonly string Name;
        public readonly string UserCode;
        public readonly bool TenantManager;
        public readonly bool Disabled;
        public readonly bool Admin;

        public ServerScriptModelGroupMemberModel(
            Context context,
            int groupId,
            string groupName,
            int deptId,
            string deptName,
            string deptCode,
            int userId,
            string loginId,
            string name,
            string userCode,
            bool tenantManager,
            bool disabled,
            bool admin)
        {
            Context = context;
            GroupId = groupId;
            GroupName = groupName;
            DeptId = deptId;
            DeptName = deptName;
            DeptCode = deptCode;
            UserId = userId;
            LoginId = loginId;
            Name = name;
            UserCode = userCode;
            TenantManager = tenantManager;
            Disabled = disabled;
            Admin = admin;
        }
    }
}