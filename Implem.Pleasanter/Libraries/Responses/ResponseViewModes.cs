using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseViewModes
    {
        public static ResponseCollection ViewMode(
            this ResponseCollection res,
            SiteSettings ss,
            View view,
            GridData gridData,
            string invoke = null,
            Message message = null,
            bool loadScroll = false,
            bool bodyOnly = false,
            string bodySelector = null,
            HtmlBuilder body = null)
        {
            return res
                .Html(!bodyOnly ? "#ViewModeContainer" : bodySelector, body)
                .View(ss: ss, view: view)
                .ReplaceAll("#Aggregations", new HtmlBuilder()
                    .Aggregations(
                        ss: ss,
                        aggregations: gridData.Aggregations))
                .ReplaceAll("#MainCommandsContainer", new HtmlBuilder()
                    .MainCommands(
                        ss: ss,
                        siteId: ss.SiteId,
                        verType: Versions.VerTypes.Latest))
                .Invoke(invoke)
                .Message(message)
                .LoadScroll(loadScroll)
                .ClearFormData();
        }
    }
}