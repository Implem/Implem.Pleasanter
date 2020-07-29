using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Linq;
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
                    href: Responses.Locations.Get(
                        context: context,
                        parts: $"resources/styles?v={extendedStyles.Sha512Cng()}"
                            + $"&site-id={context.SiteId}"
                            + $"&id={context.Id}"
                            + $"&controller={context.Controller}"
                            + $"&action={context.Action}"),
                    _using: !extendedStyles.IsNullOrEmpty());
        }

        public static string ExtendedStyles(
            Context context)
        {
            return ExtendedStyles(
                deptId: context.DeptId,
                userId: context.UserId,
                siteId: context.SiteId,
                id: context.Id,
                controller: context.Controller,
                action: context.Action);
        }

        public static string ExtendedStyles(
            int deptId,
            int userId,
            long siteId,
            long id,
            string controller,
            string action)
        {
            return ExtensionUtilities.ExtensionWhere<ExtendedStyle>(
                extensions: Parameters.ExtendedStyles,
                deptId: deptId,
                userId: userId,
                siteId: siteId,
                id: id,
                controller: controller,
                action: action)
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
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "Styles/Plugins/Normalize.css"),
                    rel: "stylesheet")
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "Styles/Plugins/jquery-ui.min.css"),
                    rel: "stylesheet")
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "Styles/Plugins/jquery.datetimepicker.min.css"),
                    rel: "stylesheet")
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "Styles/Plugins/jquery.multiselect.css"),
                    rel: "stylesheet")
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "Styles/Plugins/jquery.multiselect.filter.css"),
                    rel: "stylesheet")
                .Link(
                    href: context.VirtualPathToAbsolute("~/content/styles.min.css"),
                    rel: "stylesheet")
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "favicon.ico"),
                    rel: "shortcut icon");
        }
    }
}