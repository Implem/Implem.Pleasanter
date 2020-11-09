using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Responses
{
    public static class ResponseViewModes
    {
        public static ResponseCollection ViewMode(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            View view,
            string invoke = null,
            Message message = null,
            bool editOnGrid = false,
            bool loadScroll = false,
            bool bodyOnly = false,
            string bodySelector = null,
            HtmlBuilder body = null)
        {
            return res
                .Html(!bodyOnly ? "#ViewModeContainer" : bodySelector, body)
                .View(context: context, ss: ss, view: view)
                .Invoke("initRelatingColumnWhenViewChanged")
                .ReplaceAll(
                    "#Breadcrumb",
                    new HtmlBuilder().Breadcrumb(
                        context: context,
                        ss: ss),
                    _using: context.Controller == "items")
                .ReplaceAll("#Guide", new HtmlBuilder()
                    .Guide(
                        context: context,
                        ss: ss))
                .ReplaceAll("#CopyDirectUrlToClipboard", new HtmlBuilder()
                    .CopyDirectUrlToClipboard(
                        context: context,
                        view: view))
                .ReplaceAll("#Aggregations", new HtmlBuilder()
                    .Aggregations(
                        context: context,
                        ss: ss,
                        view: view))
                .ReplaceAll("#MainCommandsContainer", new HtmlBuilder()
                    .MainCommands(
                        context: context,
                        ss: ss,
                        verType: Versions.VerTypes.Latest,
                        backButton: !context.Publish && !editOnGrid))
                .SetMemory("formChanged", false, _using: !editOnGrid)
                .Invoke(invoke)
                .Message(message)
                .LoadScroll(loadScroll)
                .ClearFormData(
                    context: context,
                    ss: ss,
                    editOnGrid: editOnGrid);
        }

        private static ResponseCollection ClearFormData(
            this ResponseCollection res,
            Context context,
            SiteSettings ss,
            bool editOnGrid)
        {
            if (editOnGrid)
            {
                new FormDataSet(
                    context: context,
                    ss: ss)
                        .Where(o => o.Suffix.IsNullOrEmpty())
                        .ForEach(formData =>
                            formData.Data.Keys.ForEach(controlId =>
                                res.ClearFormData(
                                    controlId,
                                    type: ss.SaveViewType == SiteSettings.SaveViewTypes.None
                                        ? "ignoreView"
                                        : null)));
            }
            else
            {
                res.ClearFormData(
                    type: ss.SaveViewType == SiteSettings.SaveViewTypes.None
                        ? "ignoreView"
                        : null);
            }
            return res;
        }
    }
}