using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class RegistrationApiModel : _BaseApiModel
    {
        public int? TenantId { get; set; }
        public int? RegistrationId { get; set; }
        public int? Ver { get; set; }
        public string MailAddress { get; set; }
        public int? Invitee { get; set; }
        public string InviteeName { get; set; }
        public string LoginId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Language { get; set; }
        public string Passphrase { get; set; }
        public string Invitingflg { get; set; }
        public int? UserId { get; set; }
        public int? DeptId { get; set; }
        public int? GroupId { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public RegistrationApiModel()
        {
        }

        public override object ObjectValue(string columnName)
        {
            switch (columnName)
            {
                case "TenantId": return TenantId;
                case "RegistrationId": return RegistrationId;
                case "Ver": return Ver;
                case "MailAddress": return MailAddress;
                case "Invitee": return Invitee;
                case "InviteeName": return InviteeName;
                case "LoginId": return LoginId;
                case "Name": return Name;
                case "Password": return Password;
                case "Language": return Language;
                case "Passphrase": return Passphrase;
                case "Invitingflg": return Invitingflg;
                case "UserId": return UserId;
                case "DeptId": return DeptId;
                case "GroupId": return GroupId;
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
