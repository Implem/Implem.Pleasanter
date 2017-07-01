using Implem.Libraries.Utilities;
using Implem.Pleasanter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class Export : ISettingListItem
    {
        public int Id { get; set; }
        public string Name;
        public bool? Header;
        public List<ExportColumn> Columns;

        public Export()
        {
        }

        public Export(
            SiteSettings ss, int id, string name, bool header, IEnumerable<string> columns)
        {
            Id = id;
            Name = name;
            Header = header;
            Columns = new List<ExportColumn>();
            columns.ForEach(column =>
                Columns.Add(new ExportColumn(ss, NewColumnId(), column)));
        }

        public void Update(string name, bool header, List<ExportColumn> columns)
        {
            Name = name;
            Header = header;
            Columns = columns;
        }

        public int NewColumnId()
        {
            return Columns.Any()
                ? Columns.Max(o => o.Id) + 1
                : 1;
        }

        public Export GetRecordingData(SiteSettings ss)
        {
            var ExportSetting = new Export();
            ExportSetting.Id = Id;
            ExportSetting.Name = Name;
            ExportSetting.Header = Header == true ? null : Header;
            ExportSetting.Columns = new List<ExportColumn>();
            Columns?.ForEach(column =>
            {
                var columnDefinition = ss.ColumnDefinitionHash.Get(column.ColumnName);
                var columnSettings = ss.GetColumn(column.ColumnName);
                column.ColumnLabelText = null;
                if (column.LabelText == columnSettings.LabelText)
                {
                    column.LabelText = null;
                }
                if (column.Type == ExportColumn.Types.Text)
                {
                    column.Type = null;
                }
                if (column.Format == columnSettings.EditorFormat)
                {
                    column.Format = null;
                }
                ExportSetting.Columns.Add(column);
            });
            return ExportSetting;
        }
    }
}