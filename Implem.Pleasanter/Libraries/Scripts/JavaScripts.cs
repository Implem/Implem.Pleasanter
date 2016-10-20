using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Scripts
{
    public static class JavaScripts
    {
        public static string DataView(
            SiteSettings ss,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            switch (dataViewName)
            {
                case "burndown": return "$p.drawBurnDown();";
                case "gantt": return "$p.drawGantt();";
                case "timeseries": return "$p.drawTimeSeries();";
                case "kamban": return "$p.setKamban();";
            }
            return string.Empty;
        }
    }
}