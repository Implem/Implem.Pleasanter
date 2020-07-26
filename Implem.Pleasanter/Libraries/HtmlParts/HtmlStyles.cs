using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlStyles
    {
        public static HtmlBuilder ExtendedStyles(this HtmlBuilder hb, Context context)
        {
            var extendedStyles = ExtendedStyles(
                siteId: context.SiteId,
                id: context.Id,
                controller: context.Controller,
                action: context.Action);
            return hb
                .Link(
                    rel: "stylesheet",
                    href: Locations.Get(
                        context: context,
                        parts: $"resources/styles?v={extendedStyles.Sha512Cng()}"
                            + $"&site-id={context.SiteId}"
                            + $"&id={context.Id}"
                            + $"&controller={context.Controller}"
                            + $"&action={context.Action}"),
                    _using: !extendedStyles.IsNullOrEmpty());
        }

        public static string ExtendedStyles(
            long siteId, long id, string controller, string action)
        {
            return Parameters.ExtendedStyles
                ?.Where(o => o.SiteIdList?.Any() != true || o.SiteIdList.Contains(siteId))
                .Where(o => o.IdList?.Any() != true || o.IdList.Contains(id))
                .Where(o => o.Controllers?.Any() != true || o.Controllers.Contains(controller))
                .Where(o => o.Actions?.Any() != true || o.Actions.Contains(action))
                .Where(o => !o.Disabled)
                .Select(o => o.Style)
                .Join("\n");
        }

        public static HtmlBuilder Styles(
            this HtmlBuilder hb, Context context, SiteSettings ss, string userStyle = null)
        {
            var style = ss.GetStyleBody(
                context: context,
                peredicate: o => o.All == true);
            return hb
                .Style(
                    style: style,
                    _using: context.ContractSettings.Style != false
                        && !style.IsNullOrEmpty())
                .Style(
                    style: userStyle,
                    _using: context.ContractSettings.Style != false
                        && !userStyle.IsNullOrEmpty());
        }

        public static HtmlBuilder LinkedStyles(this HtmlBuilder hb, Context context)
        {
            return hb
                .Link(
                    href: Locations.Get(
                        context: context,
                        parts: "Styles/Plugins/Normalize.css"),
                    rel: "stylesheet")
                .Link(
                    href: Locations.Get(
                        context: context,
                        parts: "Styles/Plugins/jquery-ui.min.css"),
                    rel: "stylesheet")
                .Link(
                    href: Locations.Get(
                        context: context,
                        parts: "Styles/Plugins/jquery.datetimepicker.min.css"),
                    rel: "stylesheet")
                .Link(
                    href: Locations.Get(
                        context: context,
                        parts: "Styles/Plugins/jquery.multiselect.css"),
                    rel: "stylesheet")
                .Link(
                    href: Locations.Get(
                        context: context,
                        parts: "Styles/Plugins/jquery.multiselect.filter.css"),
                    rel: "stylesheet")
                .Link(
                    href: context.VirtualPathToAbsolute("~/content/styles.min.css"),
                    rel: "stylesheet")
                .Link(
                    href: Locations.Get(
                        context: context,
                        parts: "favicon.ico"),
                    rel: "shortcut icon");
        }
    }
}