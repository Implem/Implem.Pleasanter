using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implem.CodeDefiner.Functions.Patch
{
    internal class PatchParameters
    {
        public static void ApplyToPatch(string patchSourcePath, string parameterDir, string newVersion, string currentVersion)
        {
            var patchDir = new DirectoryInfo(patchSourcePath);
            DirectoryInfo[] dirs = patchDir.GetDirectories();
            var targetDir = dirs
                .OrderBy(o => o.Name)
                .SkipWhile(o => o.Name != currentVersion);
            var jdp = new JsonDiffPatch();
            foreach (var dir in targetDir)
            {
                if (currentVersion == dir.Name)
                {
                    continue;
                }
                foreach (var file in Directory.GetFiles(parameterDir, "*.*", SearchOption.AllDirectories))
                {
                    var fileName = Path.GetFileName(file);
                    string fileContent = null;
                    foreach (var patch in Directory.GetFiles(dir.FullName, "*.*", SearchOption.AllDirectories))
                    {
                        var patchName = Path.GetFileName(patch);
                        if (fileName == patchName)
                        {
                            if (fileContent == null)
                            {
                                fileContent = File.ReadAllText(file);
                            }
                            var patchContent = File.ReadAllText(patch);
                            var fileContentToken = JToken.Parse(fileContent);
                            var patchContentToken = JToken.Parse(patchContent);
                            fileContent = SerializeWithIndent(jdp.Patch(fileContentToken, patchContentToken));
                        }
                    }
                    if (fileContent != null)
                    {
                        File.WriteAllText(file, fileContent);
                    }
                }
                if (newVersion == dir.Name)
                {
                    var newVersionObj = new System.Version(newVersion);
                    Console.WriteLine("Patch application was successful." + newVersionObj);
                    break;
                }
            }
        }

        public static string SerializeWithIndent(object obj, int indent = 4)
        {
            var sb = new StringBuilder();
            using (var writer = new System.IO.StringWriter(sb))
            using (var jsonWriter = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented,
                Indentation = indent,
            })
            {
                new JsonSerializer().Serialize(jsonWriter, obj);
            }
            return sb.ToString();
        }
    }
}
