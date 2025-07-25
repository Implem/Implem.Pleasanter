using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class ExportColumn
    {
        public int Id;
        public string ColumnName;
        public string LabelText;
        public string ChoiceText;
        public string ChoiceValue;
        public Types? Type;
        public string Format;
        public bool? OutputClassColumn;
        [NonSerialized]
        public string SiteTitle;
        [NonSerialized]
        public Column Column;
        public long? SiteId;
        public bool ExportJsonFormat = false;

        public ExportColumn()
        {
        }

        public ExportColumn(Context context, Column column, int id = 0)
        {
            Id = id;
            SiteTitle = column.SiteSettings.Title;
            ColumnName = column.ColumnName;
            Column = column;
        }

        public enum Types
        {
            Value,
            Text,
            TextMini
        }

        public string GetColumnLabelText()
        {
            return "[" + SiteTitle + "]" + Column?.LabelText;
        }

        public string GetLabelText(bool withSiteTitle = false)
        {
            return (withSiteTitle
                ? "[" + SiteTitle + "]"
                : string.Empty)
                    + (LabelText ?? Column?.LabelText)
                        + (!ChoiceText.IsNullOrEmpty()
                            ? $"[{ChoiceText}]"
                            : string.Empty);
        }

        public new string GetType()
        {
            return (Type ?? Types.Text).ToInt().ToString();
        }

        public string GetFormat()
        {
            switch (Column?.TypeName)
            {
                case "datetime": return Format ?? Column?.EditorFormat;
                default: return null;
            }
        }

        public List<ExportColumn> NormalOrOutputClassColumns()
        {
            return OutputClassColumn == true
                && Column.HasChoices()
                && Column.ChoiceHash.Count <= Parameters.General.ExportOutputColumnMax
                    ? Column.ChoiceHash
                        .Select(choice => new ExportColumn()
                        {
                            Id = Id,
                            ColumnName = ColumnName,
                            ChoiceText = choice.Value.Text,
                            ChoiceValue = choice.Value.Value,
                            Type = Type,
                            Format = Format,
                            OutputClassColumn = OutputClassColumn,
                            SiteTitle = SiteTitle,
                            Column = Column
                        })
                        .ToList()
                    : new List<ExportColumn>() { this };
        }

        public void Update(string labelText, Types type, string format, bool outputClassColumn)
        {
            LabelText = labelText;
            Type = type;
            Format = format;
            OutputClassColumn = outputClassColumn;
        }

        public ExportColumn GetRecordingData()
        {
            var exportColumn = new ExportColumn();
            exportColumn.Id = Id;
            exportColumn.ColumnName = ColumnName;
            if (LabelText != Column?.LabelText)
            {
                exportColumn.LabelText = LabelText;
            }
            if (Type != Types.Text)
            {
                exportColumn.Type = Type;
            }
            if (ColumnName == "Comments"
                && exportColumn.Type == null)
            {
                exportColumn.Type = Types.Text;
            }
            if (!Format.IsNullOrEmpty() && Format != Column?.EditorFormat)
            {
                exportColumn.Format = Format;
            }
            if (OutputClassColumn == true)
            {
                exportColumn.OutputClassColumn = OutputClassColumn;
            }
            return exportColumn;
        }
    }
}