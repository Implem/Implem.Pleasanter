using System.Collections.Generic;
using System.IO;
using System.Text.Json.Nodes;

namespace Implem.Pleasanter.Libraries.Manifests
{
    public static class ManifestLoader
    {
        public class ResultEntry
        {
            public string File { get; set; } = "";
            public List<string>? Imports { get; set; }
        }

        public static List<ResultEntry> Load(string fileName)
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

    }
}
