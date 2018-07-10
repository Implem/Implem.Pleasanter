using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
namespace Implem.Pleasanter.Libraries.Settings
{
    public class Context
    {
        public int TenantId;
        public int UserId;
        public int DeptId;
        public string Controller;
        public string Action;
        public long Id;
        public bool HasPrivilege;

        public Context()
        {
            TenantId = Sessions.TenantId();
            UserId = Sessions.UserId();
            DeptId = Sessions.DeptId();
            Controller = Routes.Controller();
            Action = Routes.Action();
            Id = Routes.Id();
            HasPrivilege = Permissions.HasPrivilege();
        }
    }
}