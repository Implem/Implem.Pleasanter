using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.ParameterAccessor.Parts;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
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

        public class ResultEntry
        {
            public string File { get; set; } = "";
            public List<string>? Imports { get; set; }
        }

        public static List<ResultEntry> ManifestLoader(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("manifest.json not found", fileName);

            var json = File.ReadAllText(fileName);
            var manifest = JsonNode.Parse(json)?.AsObject();
            var result = new List<ResultEntry>();
            if (manifest == null) return result;

            foreach (var entry in manifest)
            {
                if (entry.Value?["isEntry"]?.GetValue<bool>() != true) continue;
                var file = entry.Value?["file"]?.ToString();
                if (string.IsNullOrEmpty(file)) continue;
                if (file.StartsWith("css/")) continue;

                List<string>? imports = null;
                if (entry.Value?["imports"] is JsonArray importArray && importArray.Count > 0)
                {
                    imports = new List<string>();
                    foreach (var import in importArray)
                    {
                        var importKey = import?.ToString();
                        if (importKey != null && manifest.ContainsKey(importKey))
                        {
                            var importFile = manifest[importKey]?["file"]?.ToString();
                            if (!string.IsNullOrEmpty(importFile))
                            {
                                imports.Add(importFile);
                            }
                        }
                    }
                    if (imports.Count == 0) imports = null;
                }
                result.Add(new ResultEntry
                {
                    File = file,
                    Imports = imports
                });
            }
            return result;
        }

        public static HtmlBuilder EsModuleLinks(
            this HtmlBuilder hb, List<ResultEntry> entries, string path, Context context)
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
                .EsModuleLinks(ManifestLoader(
                    Path.Combine(Environments.CurrentDirectoryPath, "wwwroot", "components", "manifest.json")
                ), "components", context)
                .EsModuleLinks(ManifestLoader(
                    Path.Combine(Environments.CurrentDirectoryPath, "wwwroot", "assets", "manifest.json")
                ), "assets", context);
        }
    }
}