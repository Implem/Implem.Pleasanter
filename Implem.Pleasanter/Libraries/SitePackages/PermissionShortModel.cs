using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Security;
using System.Data;
namespace Implem.Pleasanter.Libraries.SitePackages
{
    public class PermissionShortModel
    {
        public long ReferenceId;
        public int DeptId;
        public int GroupId;
        public int UserId;
        public Permissions.Types PermissionType;

        internal PermissionShortModel()
        {
        }

        internal PermissionShortModel(DataRow data)
        {
            ReferenceId = data.Long("ReferenceId");
            DeptId = data.Int("DeptId");
            GroupId = data.Int("GroupId");
            UserId = data.Int("UserId");
            PermissionType = (Permissions.Types)data.Long("PermissionType");
        }
    }
}