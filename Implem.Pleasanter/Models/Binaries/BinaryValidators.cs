using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class BinaryValidators
    {
        public static Error.Types OnGetting(Permissions.Types pt)
        {
            if (!pt.CanRead())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(Permissions.Types pt)
        {
            if (!pt.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
