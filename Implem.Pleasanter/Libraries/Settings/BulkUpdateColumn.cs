using Implem.Pleasanter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class BulkUpdateColumn : ISettingListItem
    {
        public int Id { get; set; }
        public string Title;
        public List<string> Columns;

        public BulkUpdateColumn()
        {
        }

        public BulkUpdateColumn(string title, List<string> columns)
        {
            Title = title;
            Columns = columns;
        }

        public BulkUpdateColumn(int id, string title, List<string> columns)
        {
            Id = id;
            Title = title;
            Columns = columns;
        }

        public BulkUpdateColumn GetRecordingData()
        {
            var bulkUpdateColumn = new BulkUpdateColumn();
            bulkUpdateColumn.Id = Id;
            bulkUpdateColumn.Title = Title;
            if (Columns?.Any() == true) bulkUpdateColumn.Columns = Columns;
            return bulkUpdateColumn;
        }

        public void Update(string title, List<string> columns)
        {
            Title = title;
            Columns = columns;
        }
    }
}