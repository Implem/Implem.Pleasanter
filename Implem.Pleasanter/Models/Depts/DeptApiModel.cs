using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class DeptApiModel
    {
        public int? TenantId;
        public int? DeptId;
        public int? Ver;
        public string DeptCode;
        public string DeptName;
        public string Body;
        public string Comments;
        public int? Creator;
        public int? Updator;
        public DateTime? CreatedTime;
        public DateTime? UpdatedTime;
        public bool? VerUp;
    }
}
