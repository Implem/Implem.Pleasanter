using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Implem.Pleasanter.Libraries.Settings
{
    public class MultilingualLabel
    {
        public string Language { get; set; }
        public string LabelText { get; set; }
        public string Description { get; set; }
        public string InputGuide { get; set; }
    }

    public static class MultilingualLabelExportImport
    {
        public static readonly List<string> SupportedLanguages = new List<string>
        {
            "ja", "en", "zh", "de", "ko", "es", "vn"
        };

        public static string ExportMultilingualLabels(Context context, SiteSettings ss)
        {
            var csv = new StringBuilder();

            csv.Append("ColumnName,Attributes");
            foreach (var lang in SupportedLanguages)
            {
                csv.Append($",{lang}");
            }
            csv.AppendLine();

            var columns = ss.Columns
                .Where(column => !string.IsNullOrEmpty(column.MultilingualLabelText))
                .OrderBy(column => column.No);

            foreach (var column in columns)
            {
                var labels = ParseMultilingualLabelText(column.MultilingualLabelText);
                if (labels == null || !labels.Any()) continue;

                if (HasAnyValue(labels, l => l.LabelText))
                {
                    csv.AppendLine(CreateCsvRow(column.ColumnName, "LabelText", labels, l => l.LabelText));
                }

                if (HasAnyValue(labels, l => l.Description))
                {
                    csv.AppendLine(CreateCsvRow(column.ColumnName, "Description", labels, l => l.Description));
                }

                if (HasAnyValue(labels, l => l.InputGuide))
                {
                    csv.AppendLine(CreateCsvRow(column.ColumnName, "InputGuide", labels, l => l.InputGuide));
                }
            }

            return csv.ToString();
        }

        private static List<MultilingualLabel> ParseMultilingualLabelText(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;

            try
            {
                return json.Deserialize<List<MultilingualLabel>>();
            }
            catch
            {
                return null;
            }
        }

        private static bool HasAnyValue(
            List<MultilingualLabel> labels,
            Func<MultilingualLabel, string> selector)
        {
            return labels.Any(label => !string.IsNullOrEmpty(selector(label)));
        }

        private static string CreateCsvRow(
            string columnName,
            string attribute,
            List<MultilingualLabel> labels,
            Func<MultilingualLabel, string> selector)
        {
            var row = new StringBuilder();
            row.Append(CsvValue(columnName));
            row.Append(",");
            row.Append(CsvValue(attribute));

            var labelMap = labels
                .GroupBy(l => l.Language)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var lang in SupportedLanguages)
            {
                row.Append(",");
                if (labelMap.TryGetValue(lang, out var label))
                {
                    row.Append(CsvValue(selector(label)));
                }
            }

            return row.ToString();
        }

        private static string CsvValue(object value)
        {
            if (value == null) return string.Empty;
            var str = value.ToString();

            if (str.Contains('\n') || str.Contains('\r') || str.Contains(',') || str.Contains('"'))
            {
                str = str.Replace("\"", "\"\"");
                return $"\"{str}\"";
            }

            return str;
        }

        public static ImportResult ImportMultilingualLabels(
            Context context,
            SiteSettings ss,
            byte[] csvBytes,
            string encoding)
        {
            var result = new ImportResult();

            try
            {
                var csv = new Csv(csvBytes, encoding);

                if (!ValidateHeaders(csv.Headers, result))
                {
                    return result;
                }

                var languageIndexMap = new Dictionary<string, int>();
                for (int i = 2; i < csv.Headers.Count; i++)
                {
                    var header = csv.Headers[i];
                    if (SupportedLanguages.Contains(header))
                    {
                        languageIndexMap[header] = i;
                    }
                }

                var columnDataMap = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

                foreach (var row in csv.Rows)
                {
                    if (row.Count < 2) continue;

                    var columnName = row[0];
                    var attribute = row[1];

                    if (string.IsNullOrEmpty(columnName) || string.IsNullOrEmpty(attribute))
                    {
                        continue;
                    }

                    if (!columnDataMap.ContainsKey(columnName))
                    {
                        columnDataMap[columnName] = new Dictionary<string, Dictionary<string, string>>();
                    }

                    foreach (var langEntry in languageIndexMap)
                    {
                        var lang = langEntry.Key;
                        var index = langEntry.Value;
                        var value = index < row.Count ? row[index] : string.Empty;

                        if (!string.IsNullOrEmpty(value))
                        {
                            if (!columnDataMap[columnName].ContainsKey(lang))
                            {
                                columnDataMap[columnName][lang] = new Dictionary<string, string>();
                            }

                            columnDataMap[columnName][lang][attribute] = value;
                        }
                    }
                }

                var columnHash = ss.Columns.ToDictionary(c => c.ColumnName);

                foreach (var columnEntry in columnDataMap)
                {
                    var columnName = columnEntry.Key;
                    var languageData = columnEntry.Value;

                    if (!columnHash.TryGetValue(columnName, out var column))
                    {
                        result.Warnings.Add(Displays.ImportColumnNotFound(context: context, data: columnName));
                        continue;
                    }

                    var labels = new List<MultilingualLabel>();
                    foreach (var langEntry in languageData)
                    {
                        var lang = langEntry.Key;
                        var attributes = langEntry.Value;

                        var label = new MultilingualLabel
                        {
                            Language = lang
                        };

                        if (attributes.TryGetValue("LabelText", out var labelText))
                        {
                            label.LabelText = labelText;
                        }
                        if (attributes.TryGetValue("Description", out var description))
                        {
                            label.Description = description;
                        }
                        if (attributes.TryGetValue("InputGuide", out var inputGuide))
                        {
                            label.InputGuide = inputGuide;
                        }

                        labels.Add(label);
                    }

                    column.MultilingualLabelText = labels.ToJson(formatting: Formatting.Indented)
                        .Replace("\r\n", "\n");
                    result.UpdatedCount++;
                }

                result.Success = true;
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ErrorMessage = e.Message;
                result.Exception = e;
            }

            return result;
        }

        private static bool ValidateHeaders(List<string> headers, ImportResult result)
        {
            if (headers.Count < 3)
            {
                result.ErrorType = ImportErrorType.InvalidColumnCount;
                return false;
            }

            if (headers[0] != "ColumnName")
            {
                result.ErrorType = ImportErrorType.InvalidFirstColumn;
                return false;
            }

            if (headers[1] != "Attributes")
            {
                result.ErrorType = ImportErrorType.InvalidSecondColumn;
                return false;
            }

            return true;
        }
    }

    public enum ImportErrorType
    {
        None,
        InvalidColumnCount,
        InvalidFirstColumn,
        InvalidSecondColumn,
        UnknownError
    }

    public class ImportResult
    {
        public bool Success { get; set; }
        public ImportErrorType ErrorType { get; set; } = ImportErrorType.None;
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
        public int UpdatedCount { get; set; }
        public List<string> Warnings { get; set; } = new List<string>();
    }
}
