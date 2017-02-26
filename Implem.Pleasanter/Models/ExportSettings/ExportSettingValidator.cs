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
        public static Error.Types OnExporting(SiteSettings ss)
        {
            if (!ss.CanExport())
            {
                return Error.Types.HasNotPermission;
            }
            return Error.Types.None;
        }
    }
}