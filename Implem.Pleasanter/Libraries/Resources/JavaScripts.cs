using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
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
                case "analy": return "$p.drawAnaly();";
                case "kamban": return "$p.setKamban();";
                case "imagelib": return "$p.setImageLib();";
            }
            return string.Empty;
        }

        public static ContentResultInheritance Get(Context context)
        {
            var siteId = context.QueryStrings.Long("site-id");
            var id = context.QueryStrings.Long("id");
            var controller = context.QueryStrings.Data("controller");
            var action = context.QueryStrings.Data("action");
            var siteTop = siteId == 0 && id == 0 && controller == "items" && action == "index";
            return new ContentResultInheritance
            {
                ContentType = "text/javascript",
                Content = HtmlScripts.ExtendedScripts(
                    context: context,
                    deptId: context.DeptId,
                    groups: context.Groups,
                    userId: context.UserId,
                    siteTop: siteTop,
                    siteId: siteId,
                    id: id,
                    controller: controller,
                    action: action)
            };
        }
    }
}