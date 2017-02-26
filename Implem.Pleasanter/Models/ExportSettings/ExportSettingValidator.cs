using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Models
{
    public static class ExportSettingValidator
    {
        /// <summary>
        /// Fixed:
        /// </summary>
        public static Error.Types OnUpdatingOrCreating(SiteSettings ss)
        {
            if (!ss.CanCreate() || !ss.CanUpdate())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}