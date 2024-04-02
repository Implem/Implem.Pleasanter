using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlGuides
    {
        public static HtmlBuilder Guide(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view)
        {
            return GetGuideText(
                context: context,
                ss: ss).IsNullOrEmpty()
                    ? hb.Div(id: "Guide")
                    : ss.GuideAllowExpand.ToBool()
                        ? view.GuidesReduced ?? ss.GuideExpand == "0"
                            ? hb.Div(
                                id: "Guide",
                                css: "reduced",
                                action: () => hb
                                    .Div(
                                        attributes: new HtmlAttributes()
                                            .Id("ExpandGuides")
                                            .Class("display-control")
                                            .OnClick("$p.send($(this));")
                                            .DataMethod("post"),
                                        action: () => hb
                                            .Span(css: "ui-icon ui-icon-folder-open")
                                            .Text(text: Displays.Guide(context: context) + ":")))
                            : hb.Div(id: "Guide", action: () =>
                                hb.Guide(
                                    context: context,
                                    ss: ss,
                                    useDisplayControl: true))
                        : hb.Div(id: "Guide", action: () =>
                            hb.Guide(
                                context: context,
                                ss: ss));
        }

        private static HtmlBuilder Guide(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            bool useDisplayControl = false)
        {
            var text = ConvertGuide(
                ss: ss,
                text: GetGuideText(context: context, ss: ss));
            return useDisplayControl
                ? hb.Div(
                    css: "with-icon-close",
                    action: () => hb
                        .Div(
                            attributes: new HtmlAttributes()
                                .Id("ReduceGuides")
                                .Class("display-control")
                                .OnClick("$p.send($(this));")
                                .DataMethod("post"),
                            action: () => hb
                                .Span(css: "ui-icon ui-icon-close"))
                    .Div(css: "markup", action: () => hb
                        .Text(text: text)))
                : hb.Div(action: () => hb
                    .Div(css: "markup", action: () => hb
                        .Text(text: text)));
        }

        private static string ConvertGuide(SiteSettings ss, string text)
        {
            switch (text)
            {
                case "[[GridGuide]]":
                    return ss.GridGuide;
                case "[[EditorGuide]]":
                    return ss.EditorGuide;
                case "[[CalendarGuide]]":
                    return ss.CalendarGuide;
                case "[[CrosstabGuide]]":
                    return ss.CrosstabGuide;
                case "[[GanttGuide]]":
                    return ss.GanttGuide;
                case "[[BurnDownGuide]]":
                    return ss.BurnDownGuide;
                case "[[TimeSeriesGuide]]":
                    return ss.TimeSeriesGuide;
                case "[[AnalyGuide]]":
                    return ss.AnalyGuide;
                case "[[KambanGuide]]":
                    return ss.KambanGuide;
                case "[[ImageLibGuide]]":
                    return ss.ImageLibGuide;
                default:
                    return text;
            }
        }

        private static string GetGuideText(Context context, SiteSettings ss)
        {
            switch (context.Action)
            {
                case "index":
                    return ss.GridGuide;
                case "new":
                    return ss.EditorGuide;
                case "edit":
                case "update":
                case "history":
                    return !ss.IsSite(context: context) ? ss.EditorGuide : string.Empty;
                case "calendar":
                    return ss.CalendarGuide;
                case "crosstab":
                    return ss.CrosstabGuide;
                case "gantt":
                    return ss.GanttGuide;
                case "burndown":
                    return ss.BurnDownGuide;
                case "timeseries":
                    return ss.TimeSeriesGuide;
                case "analy":
                    return ss.AnalyGuide;
                case "kamban":
                    return ss.KambanGuide;
                case "imagelib":
                    return ss.ImageLibGuide;
                default:
                    return string.Empty;
            }
        }
    }
}