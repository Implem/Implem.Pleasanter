using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Scripts
{
    public static class JavaScripts
    {
        public static string DataView(
            SiteSettings siteSettings,
            Permissions.Types permissionType,
            FormData formData,
            string dataViewName)
        {
            if (Routes.Method() == "get")
            {
                switch (dataViewName)
                {
                    case "BurnDown": return "$p.drawBurnDown();";
                    case "Gantt": return "$p.drawGantt();";
                    case "TimeSeries": return "$p.drawTimeSeries();";
                    case "Kamban": return "$p.setKamban();";
                }
            }
            return string.Empty;
        }
    }
}