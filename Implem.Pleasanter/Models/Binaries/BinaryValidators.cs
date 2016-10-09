using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Security;
namespace Implem.Pleasanter.Models
{
    public class BinaryValidators
    {
        public static Error.Types OnGetting(Permissions.Types permissionType)
        {
            if (!permissionType.CanRead())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(Permissions.Types permissionType)
        {
            if (!permissionType.CanEditSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}