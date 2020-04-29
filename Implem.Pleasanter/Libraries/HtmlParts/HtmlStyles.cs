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
            return hb
                .Link(
                    rel: "stylesheet",
                    href: Locations.Get(
                        context: context,
                        parts: "Resources/Styles?v="
                            + Parameters.ExtendedStyles.Join().Sha512Cng()),
                    _using: Parameters.ExtendedStyles?.Any() == true);
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