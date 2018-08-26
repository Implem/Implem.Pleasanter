using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class PermissionValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUpdating(Context context, SiteSettings ss)
        {
            if (!context.CanManagePermission(ss: ss))
            {
                return Error.Types.HasNotPermission;
            }
            foreach (var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "InheritPermission":
                        var type = SiteValidators.InheritPermission(
                            context: context,
                            ss: ss);
                        if (type != Error.Types.None) return type;
                        break;
                }
            }
            return Error.Types.None;
        }
    }
}
