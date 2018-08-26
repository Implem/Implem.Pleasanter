using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseViewModes
    {
        public static ResponseCollection ViewMode(
            this ResponseCollection res,
            Context context,
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
                .View(context: context, ss: ss, view: view)
                .ReplaceAll("#Breadcrumb", new HtmlBuilder()
                    .Breadcrumb(context: context, ss: ss))
                .ReplaceAll("#CopyDirectUrlToClipboard", new HtmlBuilder()
                    .CopyDirectUrlToClipboard(ss: ss))
                .ReplaceAll("#Aggregations", new HtmlBuilder()
                    .Aggregations(
                        context: context,
                        ss: ss,
                        aggregations: gridData.Aggregations))
                .ReplaceAll("#MainCommandsContainer", new HtmlBuilder()
                    .MainCommands(
                        context: context,
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