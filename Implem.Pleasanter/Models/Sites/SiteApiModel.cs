using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class SiteApiModel
    {
        public long? SiteId;
        public string Title;
        public string Body;
        public string ReferenceType;
        public long? InheritPermission;
        public string Timestamp;
        public string Comments;
        public bool? VerUp;
    }
}
