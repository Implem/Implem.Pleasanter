using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class BinaryValidators
    {
        public static Error.Types OnGetting(SiteSettings ss)
        {
            if (!ss.CanRead())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }

        public static Error.Types OnUpdating(SiteSettings ss)
        {
            if (!ss.CanManageSite())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
