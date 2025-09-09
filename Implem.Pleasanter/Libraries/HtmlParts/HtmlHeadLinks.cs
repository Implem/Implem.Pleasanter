using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Manifests;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.IO;
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

        public static HtmlBuilder EsModuleLinks(
            this HtmlBuilder hb, List<ManifestLoader.ResultEntry> entries, string path, Context context)
        {
            var linkedFiles = new HashSet<string>();
            foreach (var entry in entries)
            {
                if (entry.Imports != null && entry.Imports.Count > 0)
                {
                    foreach (var importFile in entry.Imports)
                    {
                        if (linkedFiles.Add(importFile))
                        {
                            hb.Link(
                                href: Responses.Locations.Raw(context, parts: $"{path}/{importFile}"),
                                rel: "modulepreload",
                                crossorigin: true);
                        }
                    }
                    if (linkedFiles.Add(entry.File))
                    {
                        hb.Link(
                            href: Responses.Locations.Raw(context, parts: $"{path}/{entry.File}"),
                            rel: "modulepreload",
                            crossorigin: true);
                    }
                }
            }
            return hb;
        }


        public static HtmlBuilder LinkedHeadLink(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            return hb
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "favicon.ico"),
                    rel: "shortcut icon")
                .EsModuleLinks(ManifestLoader.Load(
                    Path.Combine(Environments.CurrentDirectoryPath, "wwwroot", "components", "manifest.json")
                ), "components", context)
                .EsModuleLinks(ManifestLoader.Load(
                    Path.Combine(Environments.CurrentDirectoryPath, "wwwroot", "assets", "manifest.json")
                ), "assets", context);
        }
    }
}