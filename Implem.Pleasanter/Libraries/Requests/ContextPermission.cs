using Implem.Pleasanter.Libraries.Security;
using System.Collections.Generic;
namespace Implem.Pleasanter.Libraries.Requests
{
    public class ContextPermission
    {
        public Permissions.Types? PermissionType { get; set; }
        public Permissions.Types? ItemPermissionType { get; set; }
    }
}
