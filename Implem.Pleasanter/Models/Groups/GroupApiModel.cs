using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class GroupApiModel : _BaseApiModel
    {
        public int? TenantId { get; set; }
        public int? GroupId { get; set; }
        public int? Ver { get; set; }
        public string GroupName { get; set; }
        public string Body { get; set; }
        public bool? Disabled { get; set; }
        public bool? LdapSync { get; set; }
        public string LdapGuid { get; set; }
        public string LdapSearchRoot { get; set; }
        public DateTime? SynchronizedTime { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public List<string> GroupMembers { get; set; }
        public List<string> GroupChildren { get; set; }

        public GroupApiModel()
        {
        }

        public override object ObjectValue(string columnName)
        {
            switch (columnName)
            {
                case "TenantId": return TenantId;
                case "GroupId": return GroupId;
                case "Ver": return Ver;
                case "GroupName": return GroupName;
                case "Body": return Body;
                case "Disabled": return Disabled;
                case "LdapSync": return LdapSync;
                case "LdapGuid": return LdapGuid;
                case "LdapSearchRoot": return LdapSearchRoot;
                case "SynchronizedTime": return SynchronizedTime;
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
