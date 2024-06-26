using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
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

        private static string ExtendedStyles(Context context)
        {
            return ExtendedStyles(
                context: context,
                deptId: context.DeptId,
                groups: context.Groups,
                userId: context.UserId,
                siteTop: context.SiteTop(),
                siteId: context.SiteId,
                id: context.Id,
                controller: context.Controller,
                action: context.Action);
        }

        public static string ExtendedStyles(
            Context context,
            int deptId,
            List<int> groups,
            int userId,
            bool siteTop,
            long siteId,
            long id,
            string controller,
            string action)
        {
            var styles = (siteTop && !context.TopStyle.IsNullOrEmpty()
                ? context.TopStyle + '\n'
                : string.Empty)
                    + ExtensionUtilities.ExtensionWhere<ExtendedStyle>(
                        extensions: Parameters.ExtendedStyles,
                        name: null,
                        deptId: deptId,
                        groups: groups,
                        userId: userId,
                        siteId: siteId,
                        id: id,
                        controller: controller,
                        action: action)
                            .Select(o => o.Style)
                            .Join("\n");
            return styles;
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

        public static HtmlBuilder LinkedStyles(
            this HtmlBuilder hb, Context context, SiteSettings ss)
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
                        parts: "Styles/Plugins/lightbox.css"),
                    rel: "stylesheet")
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: context.ThemeVersionForCss() >= 2.0M && context.Mobile
                            ? $"Styles/Plugins/themes/cupertino/jquery-ui.min.css"
                            : $"Styles/Plugins/themes/{context.Theme()}/jquery-ui.min.css"),
                    rel: "stylesheet")
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: context.ThemeVersionForCss() >= 2.0M && context.Mobile
                            ? $"Styles/Plugins/themes/cupertino/custom.css"
                            : $"Styles/Plugins/themes/{context.Theme()}/custom.css"),
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
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "Scripts/Plugins/gridstack.js/gridstack.min.css"),
                    rel: "stylesheet")
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "Styles/Plugins/material-symbols-0.8.0/material-symbols/index.css"),
                    rel: "stylesheet")
                .Link(
                    href: context.VirtualPathToAbsolute($"~/content/styles.min.css?v={Environments.BundlesVersions.Get("styles.css")}"),
                    rel: "stylesheet")
                .Link(
                    href: context.VirtualPathToAbsolute($"~/content/responsive.min.css?v={Environments.BundlesVersions.Get("responsive.css")}"),
                    rel: "stylesheet",
                    _using: Parameters.Mobile.Responsive
                        && context.Mobile
                        && context.Responsive
                        && (ss == null || ss.Responsive != false))
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "Styles/responsive.custom.css"),
                    rel: "stylesheet",
                    _using: Parameters.Mobile.Responsive
                        && context.Mobile
                        && context.Responsive
                        && (ss == null || ss.Responsive != false))
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "favicon.ico"),
                    rel: "shortcut icon");
        }
    }
}