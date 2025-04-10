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
    public static class HtmlHeadLink
    {
        public static HtmlBuilder ExtendedHeadLinks(this HtmlBuilder hb, Context context)
        {
            var extendedHeadLinks = ExtendedHeadLinks(context: context);
            return hb
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: $"resources/styles?v={extendedHeadLinks.Sha512Cng()}"
                            + $"&site-id={context.SiteId}"
                            + $"&id={context.Id}"
                            + $"&controller={context.Controller}"
                            + $"&action={context.Action}"),
                    _using: !extendedHeadLinks.IsNullOrEmpty());
        }

        private static string ExtendedHeadLinks(Context context)
        {
            return ExtendedHeadLinks(
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

        public static string ExtendedHeadLinks(
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
                        extensions: Parameters.ExtendedHeadLinks,
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

        public static HtmlBuilder LinkedHeadLink(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var webComponentsHash = "2014160";
            return hb
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "favicon.ico"),
                    rel: "shortcut icon")

                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: $"scripts/plugins/components.bundle.js?v={webComponentsHash}"),
                    rel: "modulepreload",
                    crossorigin: true)
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: $"scripts/plugins/vendor.bundle.js?v={webComponentsHash}"),
                    rel: "modulepreload",
                    crossorigin: true);
        }
    }
}