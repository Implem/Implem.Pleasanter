using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class SiteApiModel
    {
        public int? TenantId;
        public long? SiteId;
        public DateTime? UpdatedTime;
        public int? Ver;
        public string Title;
        public string Body;
        public string ReferenceType;
        public long? ParentId;
        public long? InheritPermission;
        public string SiteSettings;
        public string Comments;
        public int? Creator;
        public int? Updator;
        public DateTime? CreatedTime;
        public bool? VerUp;
    }
}
