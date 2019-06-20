using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public static class PermissionValidators
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static ErrorData OnUpdating(Context context, SiteSettings ss)
        {
            if (!context.CanManagePermission(ss: ss))
            {
                return new ErrorData(type: Error.Types.HasNotPermission);
            }
            foreach (var key in context.Forms.Keys)
            {
                switch (key)
                {
                    case "InheritPermission":
                        var errorData = SiteValidators.InheritPermission(
                            context: context,
                            ss: ss);
                        if (errorData.Type != Error.Types.None) return errorData;
                        break;
                }
            }
            return new ErrorData(type: Error.Types.None);
        }
    }
}
