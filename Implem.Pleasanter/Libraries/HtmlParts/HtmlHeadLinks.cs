using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
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

        public static Dictionary<string, string> ManifestLoader(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("manifest.json not found", fileName);
            var json = File.ReadAllText(fileName);
            var manifest = JsonNode.Parse(json)?.AsObject();
            var result = new Dictionary<string, string>();
            if (manifest == null) return result;
            foreach (var entry in manifest)
            {
                var name = entry.Value?["name"]?.ToString();
                var file = entry.Value?["file"]?.ToString();
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(file))
                {
                    result[name] = file;
                }
            }
            return result;
        }

        public static HtmlBuilder LinkedHeadLink(
            this HtmlBuilder hb, Context context, SiteSettings ss)
        {
            var path = Path.Combine(Environments.CurrentDirectoryPath, "wwwroot", "components", "manifest.json");
            var json = ManifestLoader(path);
            return hb
                .Link(
                    href: Responses.Locations.Get(
                        context: context,
                        parts: "favicon.ico"),
                    rel: "shortcut icon")

                .Link(
                    href: Responses.Locations.Raw(
                        context: context,
                        parts: $"components/{json["main"]}"),
                    rel: "modulepreload",
                    crossorigin: true)
                .Link(
                    href: Responses.Locations.Raw(
                        context: context,
                        parts: $"components/{json["vendor"]}"),
                    rel: "modulepreload",
                    crossorigin: true);
        }
    }
}