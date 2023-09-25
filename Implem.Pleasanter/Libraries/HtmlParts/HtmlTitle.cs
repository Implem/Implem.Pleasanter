using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlTitle
    {
        public static HtmlBuilder Title(Context context, SiteSettings ss)
        {
            var hb = new HtmlBuilder();
            return hb.Title(action: () => hb
                .Text(text: TitleText(
                    context: context,
                    ss: ss)));
        }

        public static string TitleText(Context context, SiteSettings ss)
        {
            switch (context.Controller)
            {
                case "items":
                case "publishes":
                    if (context.Id == 0)
                    {
                        return FormattedHtmlTitle(
                            context: context,
                            ss: ss,
                            format: context.HtmlTitleTop);
                    }
                    else if (context.Id == context.SiteId)
                    {
                        return FormattedHtmlTitle(
                            context: context,
                            ss: ss,
                            format: context.HtmlTitleSite);
                    }
                    else
                    {
                        return FormattedHtmlTitle(
                            context: context,
                            ss: ss,
                            format: context.HtmlTitleRecord);
                    }
                default:
                    return FormattedHtmlTitle(
                        context: context,
                        ss: ss,
                        format: context.HtmlTitleTop);
            }
        }

        private static string FormattedHtmlTitle(
            Context context, SiteSettings ss, string format, bool publishes = false)
        {
            return context.HasPermission(ss: ss)
                ? Strings.CoalesceEmpty(
                    format?
                        .Replace("[ProductName]", Displays.ProductName(context: context))
                        .Replace("[TenantTitle]", context.TenantTitle)
                        .Replace("[SiteTitle]", context.SiteTitle)
                        .Replace("[RecordTitle]", context.CanRead(ss: ss)
                            ? context.RecordTitle
                            : Displays.ProductName(context: context))
                        .Replace("[Action]", GetActionName(context: context)),
                    context.TenantTitle,
                    Displays.ProductName(context: context))
                : Displays.ProductName(context: context);
        }

        private static string GetActionName(Context context)
        {
            switch (context.Action)
            {
                case "new":
                    return Displays.New(context: context);
                case "edit":
                    return Displays.Edit(context: context);
                case "index":
                    return Displays.Index(context: context);
                case "calendar":
                    return Displays.Calendar(context: context);
                case "crosstab":
                    return Displays.Crosstab(context: context);
                case "gantt":
                    return Displays.Gantt(context: context);
                case "burndown":
                    return Displays.BurnDown(context: context);
                case "timeseries":
                    return Displays.TimeSeries(context: context);
                case "analy":
                    return Displays.Analy(context: context);
                case "kamban":
                    return Displays.Kamban(context: context);
                case "imagelib":
                    return Displays.ImageLib(context: context);
                case "trashbox":
                    return Displays.TrashBox(context: context);
                default:
                    return string.Empty;
            }
        }
    }
}