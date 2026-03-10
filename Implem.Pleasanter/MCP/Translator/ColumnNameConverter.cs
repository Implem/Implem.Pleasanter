using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Implem.Pleasanter.MCP.Translator
{
    public class ColumnNameConverter
    {
        private readonly Dictionary<string, string> columnToLabelMap;
        private readonly Context context;
        private readonly Dictionary<string, string> labelToColumnMap;
        private readonly SiteSettings ss;

        private static readonly Dictionary<string, string> ReservedKeyMap = new Dictionary<string, string>
        {
            { "ユーザID", "UserId" },
            { "ユーザーID", "UserId" },
            { "グループ", "Groups" },
            { "グループID", "GroupId" },
            { "組織", "DeptId" },
            { "組織ID", "DeptId" }
        };

        private static readonly string[] SystemTableNames = new[]
        {
            "Depts",
            "Groups",
            "Users"
        };

        public ColumnNameConverter(
            Context context,
            SiteSettings ss)
        {
            this.context = context;
            this.ss = ss;
            this.labelToColumnMap = BuildLabelToColumnMap();
            this.columnToLabelMap = BuildColumnToLabelMap();
        }

        public string ToColumnName(string labelText)
        {
            if (string.IsNullOrEmpty(labelText))
            {
                return labelText;
            }
            if (ReservedKeyMap.TryGetValue(
                key: labelText,
                value: out var reservedKey))
            {
                return reservedKey;
            }
            if (labelToColumnMap.TryGetValue(
                key: labelText,
                value: out var columnName))
            {
                return columnName;
            }
            return labelText;
        }

        public string ToLabelText(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return columnName;
            }
            if (columnToLabelMap.TryGetValue(
                key: columnName,
                value: out var labelText))
            {
                return labelText;
            }
            return columnName;
        }

        private void AddColumnDefinitions(Dictionary<string, string> map)
        {
            var columnDefinitions = Def.ColumnDefinitionCollection
                .Where(columnDef =>
                    SystemTableNames.Contains(columnDef.TableName) ||
                    columnDef.TableName == ss?.ReferenceType)
                .Where(columnDef => !string.IsNullOrEmpty(columnDef.ColumnName))
                .ToList();
            foreach (var columnDef in columnDefinitions)
            {
                var labelText = Displays.Get(
                    context: context,
                    id: columnDef.Id);
                if (!string.IsNullOrEmpty(labelText))
                {
                    map.TryAdd(
                        key: labelText,
                        value: columnDef.ColumnName);
                }
            }
        }

        private void AddColumnDefinitionsReverse(Dictionary<string, string> map)
        {
            var columnDefinitions = Def.ColumnDefinitionCollection
                .Where(columnDef =>
                    SystemTableNames.Contains(columnDef.TableName) ||
                    columnDef.TableName == ss?.ReferenceType)
                .Where(columnDef => !string.IsNullOrEmpty(columnDef.ColumnName))
                .ToList();
            foreach (var columnDef in columnDefinitions)
            {
                var labelText = Displays.Get(
                    context: context,
                    id: columnDef.Id);
                if (!string.IsNullOrEmpty(labelText))
                {
                    map.TryAdd(
                        key: columnDef.ColumnName,
                        value: labelText);
                }
            }
        }

        private void AddSiteColumns(Dictionary<string, string> map)
        {
            var columns = ss?.Columns?
                .Where(c => !string.IsNullOrEmpty(c?.LabelText))
                .ToList();
            if (columns == null)
            {
                return;
            }
            foreach (var column in columns)
            {
                map.TryAdd(
                    key: column.LabelText,
                    value: column.ColumnName);
            }
        }

        private void AddSiteColumnsReverse(Dictionary<string, string> map)
        {
            var columns = ss?.Columns?
                .Where(c => !string.IsNullOrEmpty(c?.LabelText))
                .ToList();
            if (columns == null)
            {
                return;
            }
            foreach (var column in columns)
            {
                map.TryAdd(
                    key: column.ColumnName,
                    value: column.LabelText);
            }
        }

        private Dictionary<string, string> BuildColumnToLabelMap()
        {
            var map = new Dictionary<string, string>();
            AddSiteColumnsReverse(map: map);
            AddColumnDefinitionsReverse(map: map);
            return map;
        }

        private Dictionary<string, string> BuildLabelToColumnMap()
        {
            var map = new Dictionary<string, string>();
            AddSiteColumns(map: map);
            AddColumnDefinitions(map: map);
            return map;
        }
    }
}
