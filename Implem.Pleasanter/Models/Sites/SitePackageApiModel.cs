using Implem.Pleasanter.Libraries.SitePackages;
using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;

namespace Implem.Pleasanter.Models.Sites
{
    [Serializable]
    public class SitePackageApiModel : _BaseApiModel
    {
        public long? TargetSiteId { get; set; }
        public string SiteTitle { get; set; } = null;
        public List<SelectedSite> SelectedSites { get; set; } = new List<SelectedSite>();
        public bool IncludeSitePermission { get; set; } = false;
        public bool IncludeRecordPermission { get; set; } = false;
        public bool IncludeColumnPermission { get; set; } = false;
        public bool IncludeNotifications { get; set; } = false;
        public bool IncludeReminders { get; set; } = false;
    }
}
