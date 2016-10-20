using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Security;
namespace Implem.Pleasanter.Models
{
    public static class ExportSettingValidator
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUpdatingOrCreating(Permissions.Types pt)
        {
            if (!pt.CanCreate() || !pt.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}