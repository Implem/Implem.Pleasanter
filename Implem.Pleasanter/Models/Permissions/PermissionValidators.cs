using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class PermissionValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUpdating(SiteSettings ss)
        {
            if (!ss.CanManagePermission())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}
