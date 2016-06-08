using Implem.DefinitionAccessor;
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
                    case "BurnDown": return Def.JavaScript.DrawBurnDown;
                    case "Gantt": return Def.JavaScript.DrawGantt;
                    case "TimeSeries": return Def.JavaScript.DrawTimeSeries;
                }
            }
            return string.Empty;
        }
    }
}