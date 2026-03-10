using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Implem.CodeDefiner.Functions.AspNetMvc.CSharp
{
    internal static class RdsSplitter
    {
        private static readonly Regex FactoryPattern = new Regex(
            @"^\s+public static (\w+)ColumnCollection \1Column\(\)",
            RegexOptions.Compiled);

        public static void Split(string rdsFilePath)
        {
            if (!File.Exists(rdsFilePath))
            {
                Console.WriteLine($"[RdsSplitter] File not found: {rdsFilePath}");
                return;
            }

            var lines = File.ReadAllLines(rdsFilePath, Encoding.UTF8);
            var usingLines = ExtractUsingLines(lines);
            var tableRanges = FindTableRanges(lines);
            var namespaceLine = ExtractNamespaceLine(lines);
            var classDeclarationLine = ExtractClassDeclarationLine(lines);

            if (tableRanges.Count == 0)
            {
                Console.WriteLine("[RdsSplitter] No table sections found in Rds.cs");
                return;
            }

            var directory = Path.GetDirectoryName(rdsFilePath);

            WriteCommonFile(rdsFilePath, lines, tableRanges);

            foreach (var kvp in tableRanges.OrderBy(x => x.Value.StartLine))
            {
                var tableName = kvp.Key;
                var range = kvp.Value;
                var tableFilePath = Path.Combine(directory, $"Rds_{tableName}.cs");
                WriteTableFile(tableFilePath, lines, usingLines, namespaceLine, classDeclarationLine, tableName, range.StartLine, range.EndLine);
            }

            Console.WriteLine($"[RdsSplitter] Split Rds.cs into {tableRanges.Count + 1} files");
        }

        private static string ExtractNamespaceLine(string[] lines)
        {
            return lines
                .Select(line => line.Trim())
                .FirstOrDefault(line => line.StartsWith("namespace ", StringComparison.Ordinal))
                ?? "namespace Implem.Pleasanter.Libraries.DataSources";
        }

        private static string ExtractClassDeclarationLine(string[] lines)
        {
            var classLine = lines
                .Select(line => line.Trim())
                .FirstOrDefault(line => line.Contains("class Rds", StringComparison.Ordinal));

            if (string.IsNullOrWhiteSpace(classLine))
            {
                return "public static partial class Rds";
            }

            if (!classLine.Contains("partial", StringComparison.Ordinal))
            {
                classLine = classLine.Replace("class Rds", "partial class Rds", StringComparison.Ordinal);
            }

            return classLine;
        }

        private static List<string> ExtractUsingLines(string[] lines)
        {
            var usingLines = new List<string>();

            foreach (var line in lines)
            {
                if (line.TrimStart().StartsWith("namespace ", StringComparison.Ordinal))
                {
                    break;
                }

                if (line.TrimStart().StartsWith("using ", StringComparison.Ordinal))
                {
                    usingLines.Add(line);
                }
            }

            return usingLines;
        }

        private static Dictionary<string, (int StartLine, int EndLine)> FindTableRanges(string[] lines)
        {
            var factoryPositions = new Dictionary<string, int>();
            for (int i = 0; i < lines.Length; i++)
            {
                var match = FactoryPattern.Match(lines[i]);
                if (match.Success)
                {
                    var tableName = match.Groups[1].Value;
                    factoryPositions[tableName] = i;
                }
            }

            var tableNames = factoryPositions.OrderBy(x => x.Value).Select(x => x.Key).ToList();
            var ranges = new Dictionary<string, (int StartLine, int EndLine)>();

            for (int i = 0; i < tableNames.Count; i++)
            {
                var tableName = tableNames[i];
                var startLine = factoryPositions[tableName];
                int endLine;

                if (i < tableNames.Count - 1)
                {
                    var nextTable = tableNames[i + 1];
                    endLine = factoryPositions[nextTable] - 1;
                }
                else
                {
                    endLine = lines.Length - 3; // 最後の } } の手前
                }

                while (endLine > startLine && string.IsNullOrWhiteSpace(lines[endLine]))
                {
                    endLine--;
                }

                ranges[tableName] = (startLine, endLine);
            }

            return ranges;
        }

        private static void WriteCommonFile(string filePath, string[] lines, IReadOnlyDictionary<string, (int StartLine, int EndLine)> tableRanges)
        {
            var sb = new StringBuilder();
            var excludeRanges = tableRanges.Values.ToList();

            for (int i = 0; i < lines.Length; i++)
            {
                if (excludeRanges.Any(range => i >= range.StartLine && i <= range.EndLine))
                {
                    continue;
                }

                var line = lines[i];
                if (line.Contains("public static class Rds"))
                {
                    line = line.Replace("public static class Rds", "public static partial class Rds");
                }
                sb.AppendLine(line);
            }

            File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
            Console.WriteLine($"[RdsSplitter] Written: {Path.GetFileName(filePath)}");
        }

        private static void WriteTableFile(
            string filePath,
            string[] lines,
            IReadOnlyList<string> usingLines,
            string namespaceLine,
            string classDeclarationLine,
            string tableName,
            int startLine,
            int endLine)
        {
            var sb = new StringBuilder();

            foreach (var usingLine in usingLines)
            {
                sb.AppendLine(usingLine);
            }
            sb.AppendLine(namespaceLine);
            sb.AppendLine("{");
            sb.AppendLine($"    {classDeclarationLine}");
            sb.AppendLine("    {");

            for (int i = startLine; i <= endLine; i++)
            {
                sb.AppendLine(lines[i]);
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
            Console.WriteLine($"[RdsSplitter] Written: {Path.GetFileName(filePath)}");
        }
    }
}
