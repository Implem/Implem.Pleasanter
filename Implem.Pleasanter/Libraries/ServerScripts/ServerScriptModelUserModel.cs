using Implem.Pleasanter.Libraries.Requests;
namespace Implem.Pleasanter.Libraries.ServerScripts
{
    public class ServerScriptModelUserModel
    {
        private readonly Context Context;
        private readonly int TenantId;
        public readonly int UserId;
        public readonly int DeptId;
        public readonly string LoginId;
        public readonly string Name;
        public readonly string UserCode;
        public readonly bool TenantManager;
        public readonly bool ServiceManager;
        public readonly bool Disabled;

        public ServerScriptModelUserModel(
            Context context,
            int tenantId,
            int userId,
            int deptId,
            string loginId,
            string name,
            string userCode,
            bool tenantManager,
            bool serviceManager,
            bool disabled)
        {
            Context = context;
            TenantId = tenantId;
            UserId = userId;
            DeptId = deptId;
            LoginId = loginId;
            Name = name;
            UserCode = userCode;
            TenantManager = tenantManager;
            ServiceManager = serviceManager;
            Disabled = disabled;
        }
    }
}