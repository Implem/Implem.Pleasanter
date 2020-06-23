using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.Libraries.Resources
{
    public static class JavaScripts
    {
        public static string ViewMode(string viewMode)
        {
            switch (viewMode)
            {
                case "index": return "$p.paging('#Grid')";
                case "calendar": return "$p.setCalendar();";
                case "crosstab": return "$p.setCrosstab();";
                case "gantt": return "$p.drawGantt();";
                case "burndown": return "$p.drawBurnDown();";
                case "timeseries": return "$p.drawTimeSeries();";
                case "kamban": return "$p.setKamban();";
                case "imagelib": return "$p.setImageLib();";
            }
            return string.Empty;
        }

        public static ContentResult Get(Context context)
        {
            return new ContentResult
            {
                ContentType = "text/javascript",
                Content = HtmlScripts.ExtendedScripts(context: context)
            };
        }
    }
}