using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Linq;
using System.Web.Optimization;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlStyles
    {
        public static HtmlBuilder ExtendedStyles(this HtmlBuilder hb, Context context)
        {
            var extendedStyles = ExtendedStyles(context: context);
            return hb
                .Link(
                    rel: "stylesheet",
                    href: Locations.Get(
                        context: context,
                        parts: "Resources/Styles?v="
                            + extendedStyles?.Sha512Cng()),
                    _using: !extendedStyles.IsNullOrEmpty());
        }

        public static string ExtendedStyles(Context context)
        {
            return Parameters.ExtendedStyles
                ?.Where(o => o.SiteIdList?.Any() != true || o.SiteIdList.Contains(context.SiteId))
                .Where(o => o.IdList?.Any() != true || o.IdList.Contains(context.Id))
                .Where(o => o.Controllers?.Any() != true || o.Controllers.Contains(context.Controller))
                .Where(o => o.Actions?.Any() != true || o.Actions.Contains(context.Action))
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
                    href: BundleTable.Bundles.ResolveBundleUrl("~/content/styles"),
                    rel: "stylesheet")
                .Link(
                    href: Locations.Get(
                        context: context,
                        parts: "favicon.ico"),
                    rel: "shortcut icon");
        }
    }
}