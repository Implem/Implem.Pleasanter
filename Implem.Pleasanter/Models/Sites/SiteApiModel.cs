using Implem.Pleasanter.Models.Shared;
using System;
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
        public string GridGuide { get; set; }
        public string EditorGuide { get; set; }
        public string ReferenceType { get; set; }
        public long? ParentId { get; set; }
        public long? InheritPermission { get; set; }
        public string SiteSettings { get; set; }
        public bool? Publish { get; set; }
        public DateTime? LockedTime { get; set; }
        public int? LockedUser { get; set; }
        public DateTime? ApiCountDate { get; set; }
        public int? ApiCount { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string ItemTitle { get; set; }

        public SiteApiModel()
        {
        }
    }
}
