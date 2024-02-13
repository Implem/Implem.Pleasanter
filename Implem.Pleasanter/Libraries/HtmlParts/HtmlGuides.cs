using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlGuides
    {
        public static HtmlBuilder Guide(this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb.Div(id: "Guide", action: () =>
            {
                switch (context.Action)
                {
                    case "index":
                        hb.Guide(
                            ss: ss,
                            text: ss.GridGuide);
                        break;
                    case "new":
                        hb.Guide(
                            ss: ss,
                            text: ss.EditorGuide);
                        break;
                    case "edit":
                    case "update":
                    case "history":
                        hb.Guide(
                            ss: ss,
                            text: ss.EditorGuide,
                            _using: !ss.IsSite(context: context));
                        break;
                    case "calendar":
                        hb.Guide(
                            ss: ss,
                            text: ss.CalendarGuide);
                        break;
                    case "crosstab":
                        hb.Guide(
                            ss: ss,
                            text: ss.CrosstabGuide);
                        break;
                    case "gantt":
                        hb.Guide(
                            ss: ss,
                            text: ss.GanttGuide);
                        break;
                    case "burndown":
                        hb.Guide(
                            ss: ss,
                            text: ss.BurnDownGuide);
                        break;
                    case "timeseries":
                        hb.Guide(
                            ss: ss,
                            text: ss.TimeSeriesGuide);
                        break;
                    case "analy":
                        hb.Guide(
                            ss: ss,
                            text: ss.AnalyGuide);
                        break;
                    case "kamban":
                        hb.Guide(
                            ss: ss,
                            text: ss.KambanGuide);
                        break;
                    case "imagelib":
                        hb.Guide(
                            ss: ss,
                            text: ss.ImageLibGuide);
                        break;
                }
            });
        }

        private static HtmlBuilder Guide(
            this HtmlBuilder hb,
            SiteSettings ss,
            string text,
            bool _using = true)
        {
            text = ConvertGuide(
                ss: ss,
                text: text);
            return _using && !text.IsNullOrEmpty()
                ? hb.Div(action: () => hb
                    .Div(css: "markup", action: () => hb
                        .Text(text: text)))
                : hb;
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
    }
}