using Implem.Libraries.Utilities;
using System;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Settings
{
    [Serializable]
    public class Link
    {
        public string ColumnName;
        public long SiteId;
        public bool? NoAddButton;
        public bool? AddSource;
        [NonSerialized]
        public string SiteTitle;
        [NonSerialized]
        public long SourceId;

        public Link()
        {
        }

        public Link(string columnName, long siteId)
        {
            ColumnName = columnName;
            SiteId = siteId;
        }

        public Link(string columnName, string settings)
        {
            ColumnName = columnName;
            settings?
                .RegexFirst(@"(?<=\[\[).+(?=\]\])")?
                .Split(',')
                .Select((o, i) => new { Index = i, Setting = o })
                .ForEach(data =>
                {
                    if (data.Index == 0)
                    {
                        SiteId = data.Setting.ToLong();
                    }
                    else
                    {
                        switch (data.Setting)
                        {
                            case "NoAddButton":
                                NoAddButton = true;
                                break;
                            case "AddSource":
                                AddSource = true;
                                break;
                        }
                    }
                });
        }

        public string LinkedTableName()
        {
            return $"{ColumnName}~{SiteId}";
        }

        public Link GetRecordingData()
        {
            var link = new Link();
            link.ColumnName = ColumnName;
            link.SiteId = SiteId;
            if (NoAddButton == true) link.NoAddButton = true;
            if (AddSource == true) link.AddSource = true;
            return link;
        }
    }
}