using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class DeptApiModel : _BaseApiModel
    {
        public int? TenantId { get; set; }
        public int? DeptId { get; set; }
        public int? Ver { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string Body { get; set; }
        public bool? Disabled { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public DeptApiModel()
        {
        }

        public override object ObjectValue(string columnName)
        {
            switch (columnName)
            {
                case "TenantId": return TenantId;
                case "DeptId": return DeptId;
                case "Ver": return Ver;
                case "DeptCode": return DeptCode;
                case "DeptName": return DeptName;
                case "Body": return Body;
                case "Disabled": return Disabled;
                case "Comments": return Comments;
                case "Creator": return Creator;
                case "Updator": return Updator;
                case "CreatedTime": return CreatedTime;
                case "UpdatedTime": return UpdatedTime;
                default: return base.ObjectValue(columnName: columnName);
            }
        }
    }
}
