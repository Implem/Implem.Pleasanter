using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class GroupApiModel
    {
        public int? TenantId;
        public int? GroupId;
        public int? Ver;
        public string GroupName;
        public string Body;
        public string Comments;
        public int? Creator;
        public int? Updator;
        public DateTime? CreatedTime;
        public DateTime? UpdatedTime;
        public bool? VerUp;
    }
}
