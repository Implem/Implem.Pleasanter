using Implem.Pleasanter.Libraries.Requests;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelGroupMemberModel
    {
        private readonly Context Context;
        public readonly int GroupId;
        public readonly int DeptId;
        public readonly int UserId;
        public readonly bool Admin;

        public ServerScriptModelGroupMemberModel(
            Context context,
            int groupId,
            int deptId,
            int userId,
            bool admin)
        {
            Context = context;
            GroupId = groupId;
            DeptId = deptId;
            UserId = userId;
            Admin = admin;
        }
    }
}