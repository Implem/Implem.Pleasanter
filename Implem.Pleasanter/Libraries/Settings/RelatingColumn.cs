using Implem.Pleasanter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class RelatingColumn : ISettingListItem
    {
        public int Id { get; set; }
        public string Title;
        public List<string> Columns;
        public Dictionary<string, string> ColumnsLinkedClass;

        public RelatingColumn()
        {
        }

        public RelatingColumn(string title, List<string> columns)
        {
            ColumnsLinkedClass = new Dictionary<string, string>();
            Title = title;
            Columns = columns;
        }

        public RelatingColumn(int id, string title, List<string> columns)
        {
            ColumnsLinkedClass = new Dictionary<string, string>();
            Id = id;
            Title = title;
            Columns = columns;
        }

        public RelatingColumn GetRecordingData()
        {
            var link = new RelatingColumn();
            link.Id = Id;
            link.Title = Title;
            if (Columns?.Any() == true) link.Columns = Columns;
            return link;
        }

        public void Update(string title, List<string> columns)
        {
            Title = title;
            Columns = columns;
        }
    }
}