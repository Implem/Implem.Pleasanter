using Implem.Pleasanter.Interfaces;
using Implem.Pleasanter.Libraries.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class Export : ISettingListItem
    {
        public enum Types : int
        {
            Csv = 0,
            Json = 1
        }
        public enum ExecutionTypes : int
        {
            Direct = 0,
            MailNotify =1
        }

        public int Id { get; set; }
        public string Name;
        public bool? Header;
        public List<ExportColumn> Columns;
        public Types Type;
        public ExecutionTypes ExecutionType;
        // compatibility Version 1.014
        public Join Join;

        public Export()
        {
        }

        public Export(
            int id,
            string name,
            Types type,
            bool header,
            List<ExportColumn> columns,
            ExecutionTypes executionType)
        {
            Id = id;
            Name = name;
            Type = type;
            Header = header;
            Columns = new List<ExportColumn>();
            columns.ForEach(column =>
            {
                column.Id = NewColumnId();
                Columns.Add(column);
            });
            ExecutionType = executionType;
        }

        public Export(List<ExportColumn> columns)
        {
            Header = true;
            Columns = new List<ExportColumn>();
            columns.ForEach(column =>
            {
                column.Id = NewColumnId();
                Columns.Add(column);
            });
        }

        public void SetColumns(Context context, SiteSettings ss)
        {
            Header = Header ?? true;
            Columns.ForEach(exportColumn =>
            {
                var column = ss.GetColumn(
                    context: context,
                    columnName: exportColumn.ColumnName);
                if (column != null)
                {
                    exportColumn.Column = column;
                    exportColumn.SiteTitle = ss.GetJoinedSs(column.TableName()).Title;
                }
            });
        }

        public void Update(
            string name,
            Types type,
            bool header,
            List<ExportColumn> columns,
            ExecutionTypes executionType)
        {
            Name = name;
            Type = type;
            Header = header;
            Columns = columns;
            ExecutionType = executionType;
        }

        public int NewColumnId()
        {
            return Columns.Any()
                ? Columns.Max(o => o.Id) + 1
                : 1;
        }

        public Export GetRecordingData()
        {
            var export = new Export();
            export.Id = Id;
            export.Name = Name;
            export.Header = Header == true ? null : Header;
            export.Columns = new List<ExportColumn>();
            export.Type = Type;
            Columns?.ForEach(column => export.Columns.Add(column.GetRecordingData()));
            export.ExecutionType = ExecutionType;
            return export;
        }
    }
}