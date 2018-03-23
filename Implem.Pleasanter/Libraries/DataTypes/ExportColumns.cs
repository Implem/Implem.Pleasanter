using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
namespace Implem.Pleasanter.Libraries.DataTypes
{
    public class ExportColumns
    {
        public string ReferenceType;
        public Dictionary<string, bool> Columns = new Dictionary<string, bool>();

        public ExportColumns()
        {
        }

        public ExportColumns(string referenceType)
        {
            ReferenceType = referenceType;
            Init();
        }

        public ExportColumns(ExportColumns source)
        {
            ReferenceType = source.ReferenceType;
            Columns = source.Columns;
        }

        private void Init()
        {
            UpdateExportSetting();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            Init();
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext streamingContext)
        {
        }

        public string ToJson()
        {
            return Jsons.ToJson(new ExportColumns(this));
        }

        private void UpdateExportSetting()
        {
            Columns.AddRange(
                Def.ColumnDefinitionCollection
                    .Where(o => !Columns.Any(p => p.Key == o.ColumnName))
                    .Where(o => o.TableName == ReferenceType)
                    .Where(o => o.Export > 0)
                    .OrderBy(o => o.Export)
                    .ToDictionary(o => o.ColumnName, o => true));
            Columns.RemoveAll((key, value) =>
                !Def.ColumnDefinitionCollection.Any(p => p.ColumnName == key && p.Export > 0));
        }

        public Dictionary<string, ControlData> ExportColumnHash(SiteSettings ss)
        {
            return Columns.ToDictionary(
                o => o.Key,
                o => new ControlData(Displays.Get(ExportColumn(ss, o.Key)) +
                    (o.Value ? " (" + Displays.Output() + ")" : string.Empty)));
        }

        public string ExportColumn(SiteSettings ss, string columnName)
        {
            return ss.Columns
                .FirstOrDefault(o => o.ColumnName == columnName && o.Export).LabelText;
        }

        public void SetExport(
            Responses.ResponseCollection res,
            string controlId,
            IEnumerable<string> selectedValues,
            SiteSettings ss)
        {
            var order = Columns.Keys.ToArray();
            if (controlId == "ColumnToDown") Array.Reverse(order);
            order.Select((o, i) => new { ColumnName = o, Index = i }).ForEach(data =>
            {
                if (selectedValues.Contains(data.ColumnName))
                {
                    switch (controlId)
                    {
                        case "ColumnToVisible":
                            Columns[data.ColumnName] = true;
                            break;
                        case "ColumnToHide":
                            Columns[data.ColumnName] = false;
                            break;
                        case "ColumnToUp":
                        case "ColumnToDown":
                            if (data.Index > 0 &&
                                !selectedValues.Contains(order[data.Index - 1]))
                            {
                                order = Arrays.Swap(
                                    order,
                                    data.Index,
                                    data.Index - 1);
                            }
                            break;
                    }
                }
            });
            if (controlId == "ColumnToDown") Array.Reverse(order);
            var newColumns = order.ToDictionary(o => o, o => Columns[o]);
            Columns.Clear();
            Columns.AddRange(newColumns);
            res.Html("#ExportSettings_Columns",
                new HtmlBuilder().SelectableItems(
                    listItemCollection: ExportColumnHash(ss),
                    selectedValueTextCollection: selectedValues));
        }

        public Dictionary<string, Column> ColumnHash(SiteSettings ss)
        {
            return Columns.ToDictionary(o => o.Key, o => ss.GetColumn(o.Key));
        }

        public bool InitialValue()
        {
            return ToJson() == "[]";
        }
    }
}