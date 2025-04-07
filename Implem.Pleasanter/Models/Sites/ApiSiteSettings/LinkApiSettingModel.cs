using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.ApiSiteSettings
{
    [Serializable]
    public class LinkApiSettingModel
    {
        public string TableName;
        public string ColumnName;
        public long SiteId;
        public int? Priority;
        public bool? NoAddButton;
        public bool? AddSource;
        public bool? NotReturnParentRecord;
        public bool? ExcludeMe;
        public bool? MembersOnly;
        public bool? SelectNewLink;
        public string SearchFormat;
        public View View;
        public Lookups Lookups;
        public LinkActions LinkActions;
        public bool? JsonFormat;
        [NonSerialized]
        public string SiteTitle;
        [NonSerialized]
        public long SourceId;

        public LinkApiSettingModel()
        {
        }
    }
}