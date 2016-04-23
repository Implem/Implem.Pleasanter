using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp.Parts
{
    internal static class VisualStudioProject
    {
        internal static void Update()
        {
            var fileInfo = new DirectoryInfo(
                Directories.Outputs())
                    .EnumerateFiles("*.csproj")
                    .FirstOrDefault();
            if (fileInfo != null)
            {
                UpdateCompilesOfProjectFile(fileInfo.FullName);
            }
        }

        private static void UpdateCompilesOfProjectFile(string filePath)
        {
            var projectCode = Files.Read(filePath);
            var itemGroupCodeBlock = projectCode.RegexFirst(
                @"(?<=<ItemGroup>\r?\n) *<Compile.*?(?=\n *</ItemGroup>)");
            var itemGroupCodeBlockNew = itemGroupCodeBlock.RegexMatches(
                "    <(Compile|Content).+?/>|    <(Compile|Content)(?:.|\n)+?</(Compile|Content)>",
                RegexOptions.Multiline)
                    .Cast<Match>()
                    .Select(o => o.Value)
                    .Where(o =>
                        !o.StartsWith("    <Compile Include=\"Models\\") &&
                        !o.StartsWith("    <Compile Include=\"Controllers\\"))
                    .Union(ModelCompileSection())
                    .Union(ControllerCompileSection())
                    .OrderBy(o => o)
                    .Join("\n");
            if (itemGroupCodeBlock != itemGroupCodeBlockNew)
            {
                projectCode.Replace(itemGroupCodeBlock, itemGroupCodeBlockNew).Write(filePath);
            }
        }

        private static IEnumerable<string> ModelCompileSection()
        {
            return Def.ColumnDefinitionCollection
                .Where(o => !o.Base)
                .Select(o => o.ModelName)
                .Union(Parameters.General.ProjectModelRequire.Split(','))
                .Select(o => o.Trim())
                .Where(o => o != string.Empty)
                .Distinct()
                .Select(o => "    <Compile Include=\"Models\\{0}Model.cs\" />".Params(o));
        }

        private static IEnumerable<string> ControllerCompileSection()
        {
            return new DirectoryInfo(Directories.Outputs("Controllers")).GetFiles("*.cs")
                .Select(o => o.Name)
                .Where(o => o != string.Empty)
                .Distinct()
                .Select(o => "    <Compile Include=\"Controllers\\{0}\" />".Params(o));
        }
    }
}
