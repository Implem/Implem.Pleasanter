using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    public static class ImportUtilities
    {
        public static Dictionary<int, ImportColumn> GetColumnHash(SiteSettings ss, Csv csv)
        {
            var columnHash = new Dictionary<int, ImportColumn>();
            csv.Headers
                .Select((o, i) => new
                {
                    LabelText = o,
                    LabelTextParts = o.Contains("[") && o.EndsWith("]")
                        ? new
                        {
                            LabelText = o.Split('[').Take(o.Split('[').Length - 1).Join("["),
                            Value = o.Substring(0, o.Length - 1).Split('[').Last()
                        }
                        : null,
                    Index = i
                })
                .ForEach(header =>
                {
                    var column = ss.Columns
                        .Where(o => o.LabelText == header.LabelText
                            || o.LabelText == header.LabelTextParts?.LabelText
                                && o.ChoiceHash?.Any(p =>
                                    p.Key == header.LabelTextParts.Value
                                    || p.Value.Text == header.LabelTextParts.Value) == true)
                        .Where(o => o.TypeCs != "Attachments")
                        .FirstOrDefault();
                    if (column != null)
                    {
                        var importColumn = new ImportColumn()
                        {
                            Column = column
                        };
                        if (column.LabelText != header.LabelText)
                        {
                            importColumn = columnHash.FirstOrDefault(o =>
                                o.Value.Column.LabelText == header.LabelTextParts.LabelText
                                && o.Value.ValueIndexes != null).Value
                                    ?? importColumn;
                            if (importColumn.ValueIndexes == null)
                            {
                                importColumn.ValueIndexes = new Dictionary<int, string>();
                            }
                            importColumn.ValueIndexes.AddIfNotConainsKey(
                                header.Index,
                                column
                                    .ChoiceHash
                                    .FirstOrDefault(o => o.Key == header.LabelTextParts.Value
                                        || o.Value.Text == header.LabelTextParts.Value)
                                    .Key);
                        }
                        columnHash.Add(header.Index, importColumn);
                    }
                });
            return columnHash
                .GroupBy(o => o.Value.Column.ColumnName)
                .Select(o => o.Last())
                .ToDictionary(o => o.Key, o => o.Value);
        }

        public static string RecordingData(
            Dictionary<int, ImportColumn> columnHash,
            List<string> row,
            KeyValuePair<int, ImportColumn> column)
        {
            if (column.Value.ValueIndexes != null)
            {
                var data = columnHash
                    .FirstOrDefault(o => o.Value.Column.ColumnName == column.Value.Column.ColumnName)
                    .Value.ValueIndexes
                    .Where(o => row.Count > o.Key && row[o.Key].ToBool())
                    .Select(o => o.Value);
                return column.Value.Column.MultipleSelections == true
                    ? data.ToJson()
                    : data.FirstOrDefault() ?? string.Empty;
            }
            else
            {
                return row.Count > column.Key
                    ? row[column.Key]
                    : string.Empty;
            }
        }

        public static int SetOnImportingExtendedSqls(Context context, SiteSettings ss)
        {
            return Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: new List<SqlStatement>()
                    .OnImportingExtendedSqls(
                        context: context,
                        siteId: ss.SiteId)
                            .ToArray());
        }

        public static int SetOnImportedExtendedSqls(Context context, SiteSettings ss)
        {
            return Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: new List<SqlStatement>()
                    .OnImportedExtendedSqls(
                        context: context,
                        siteId: ss.SiteId)
                            .ToArray());
        }

        
    }
}
