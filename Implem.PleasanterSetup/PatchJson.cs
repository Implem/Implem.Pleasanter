using JsonDiffPatchDotNet.Formatters.JsonPatch;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;
using System.Text;
using DiffMatchPatch;

namespace Implem.PleasanterSetup
{
    internal static class PatchJson
    {
        public static string CreatePatch(string release, string previous, string createPath)
        {
            //var jdp = new JsonDiffPatch(new Options() { DiffBehaviors = DiffBehavior.IgnoreNewProperties | DiffBehavior.IgnoreMissingProperties});
            var jdp = new JsonDiffPatch();
            var previousToken = JToken.Parse(previous);
            var releaseToken = JToken.Parse(release);
            var patch = jdp.Diff(previousToken, releaseToken);
            //var formatter = new JsonDeltaFormatter();
            //var operations = formatter.Format(patch);
            //JToken operation = JArray.FromObject(operations);
            //op["op"]?.ToString() != "replace"が存在する場合は実行、存在しない場合はpatchファイルを作成しない
            //foreach (var op in operation)
            //{
            //    if (op["op"]?.ToString() == "replace")
            //    {

            //        var path = op["path"]?.ToString().Replace("/", ".").TrimStart('.');
            //        JToken p = patch?.SelectToken($"$.{path}");
            //        p?.Parent?.Remove();
            //    }
            //}
            Console.WriteLine(patch);
            return JsonConvert.SerializeObject(patch, Formatting.Indented);
        }

        public static void ApplyToPatch(string installDir,string destPath,string newVesion, string currentVersion)
        {
            //最新バージョンに同梱されているPleasanterPatchフォルダを参照する
            var patchSource = Path.Combine(
                    installDir,
                    "PleasanterPatch");
            var patchDir = new DirectoryInfo(patchSource);
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
                foreach (var file in Directory.GetFiles(destPath, "*.*", SearchOption.AllDirectories))
                {
                    var fileName = Path.GetFileName(file);
                    string fileContent = null;
                    foreach (var patch in Directory.GetFiles(dir.FullName, "*.*", SearchOption.AllDirectories))
                    {
                        var patchName = Path.GetFileName(patch);
                        if (fileName == patchName)
                        {
                            if(fileContent == null)
                            {
                                fileContent = File.ReadAllText(file);
                            }
                            var patchContent = File.ReadAllText(patch);
                            var fileContentToken = JToken.Parse(fileContent);
                            var patchContentToken = JToken.Parse(patchContent);
                            fileContent = SerializeWithIndent(jdp.Patch(fileContentToken, patchContentToken));
                        }
                    }
                    if(fileContent != null)
                    {
                        File.WriteAllText(file, fileContent);
                    }
                }
                if (newVesion == dir.Name)
                {
                    Console.WriteLine("Patch application was successful." + dir.Name);
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
