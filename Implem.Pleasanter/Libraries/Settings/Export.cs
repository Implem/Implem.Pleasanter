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
        public Join Join;
        public List<ExportColumn> Columns;

        public Export()
        {
        }

        public Export(int id, string name, bool header, List<ExportColumn> columns)
        {
            Id = id;
            Name = name;
            Header = header;
            Columns = new List<ExportColumn>();
            columns.ForEach(column =>
            {
                column.Id = NewColumnId();
                Columns.Add(column);
            });
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

        public void Update(
            string name,
            bool header,
            Join join,
            List<ExportColumn> columns)
        {
            Name = name;
            Header = header;
            Join = join;
            Columns = columns;
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
            export.Join = Join?.Any() == true ? Join : null;
            export.Columns = new List<ExportColumn>();
            Columns?.ForEach(column => export.Columns.Add(column.GetRecordingData()));
            return export;
        }
    }
}