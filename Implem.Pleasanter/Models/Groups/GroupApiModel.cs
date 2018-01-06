using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class GroupApiModel
    {
        public int? TenantId;
        public int? GroupId;
        public string GroupName;
        public string Body;
        public string Timestamp;
        public string Comments;
        public bool? VerUp;
    }
}
