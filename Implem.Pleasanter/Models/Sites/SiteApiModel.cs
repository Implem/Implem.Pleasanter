using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class SiteApiModel : _BaseApiModel
    {
        public int? TenantId { get; set; }
        public long? SiteId { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public int? Ver { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string SiteName { get; set; }
        public string SiteGroupName { get; set; }
        public string GridGuide { get; set; }
        public string EditorGuide { get; set; }
        public string CalendarGuide { get; set; }
        public string CrosstabGuide { get; set; }
        public string GanttGuide { get; set; }
        public string BurnDownGuide { get; set; }
        public string TimeSeriesGuide { get; set; }
        public string AnalyGuide { get; set; }
        public string KambanGuide { get; set; }
        public string ImageLibGuide { get; set; }
        public string ReferenceType { get; set; }
        public long? ParentId { get; set; }
        public long? InheritPermission { get; set; }
        public List<string> Permissions { get; set; }
        public SiteSettings SiteSettings { get; set; }
        public bool? Publish { get; set; }
        public bool? DisableCrossSearch { get; set; }
        public DateTime? LockedTime { get; set; }
        public int? LockedUser { get; set; }
        public DateTime? ApiCountDate { get; set; }
        public int? ApiCount { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string ItemTitle { get; set; }
        public bool? DisableSiteCreatorPermission { get; set; }
        public List<int> SummaryId { get; set; }
        public List<string> FindSiteNames { get; set; }

        public SiteApiModel()
        {
        }
    }
}
