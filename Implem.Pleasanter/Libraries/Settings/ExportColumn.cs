using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using System;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class ExportColumn
    {
        public int Id;
        public string ColumnName;
        public string LabelText;
        public Types? Type;
        public string Format;
        [NonSerialized]
        public string SiteTitle;
        [NonSerialized]
        public Column Column;
        // compatibility Version 1.014
        public long? SiteId;

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
                    + (LabelText ?? Column?.LabelText);
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

        public void Update(string labelText, Types type, string format)
        {
            LabelText = labelText;
            Type = type;
            Format = format;
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
            if (!Format.IsNullOrEmpty() && Format != Column?.EditorFormat)
            {
                exportColumn.Format = Format;
            }
            return exportColumn;
        }
    }
}