using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class DeptApiModel
    {
        public int? DeptId;
        public string DeptCode;
        public string DeptName;
        public string Body;
        public string Timestamp;
        public string Comments;
        public bool? VerUp;
    }
}
