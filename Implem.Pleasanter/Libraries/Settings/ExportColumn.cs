using Implem.Libraries.Utilities;
using System;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class ExportColumn
    {
        public int Id;
        public string ColumnName;
        public string ColumnLabelText;
        public string LabelText;
        public Types? Type;
        public string Format;

        public ExportColumn()
        {
        }

        public ExportColumn(SiteSettings ss, int id, string columnName)
        {
            Id = id;
            ColumnName = columnName;
            Init(ss);
        }

        public enum Types
        {
            Value,
            Text,
            TextMini
        }

        public void Init(SiteSettings ss)
        {
            var columnDefinition = ss.ColumnDefinitionHash.Get(ColumnName);
            var columnSettings = ss.GetColumn(ColumnName);
            ColumnLabelText = columnSettings.LabelText;
            LabelText = LabelText ?? columnSettings.LabelText;
            Type = Type ?? Types.Text;
            Format = columnDefinition.TypeName == "datetime"
                ? Format ?? columnSettings.EditorFormat
                : null;
        }
    }
}