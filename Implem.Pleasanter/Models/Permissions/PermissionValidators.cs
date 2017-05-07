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
            foreach (var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "InheritPermission":
                        if (!ss.CanManagePermission())
                        {
                            return Error.Types.HasNotPermission;
                        }
                        var inheritPermission = Forms.Long(controlId);
                        if (ss.SiteId != inheritPermission &&
                            !Permissions.CanRead(inheritPermission))
                        {
                            return Error.Types.HasNotPermission;
                        }
                        if (PermissionUtilities.HasInheritedSites(ss.SiteId))
                        {
                            return Error.Types.CanNotChangeInheritance;
                        }
                        break;
                }
            }
            return Error.Types.None;
        }
    }
}
