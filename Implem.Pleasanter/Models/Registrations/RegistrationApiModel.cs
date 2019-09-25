using Implem.Pleasanter.Models.Shared;
using System;
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
    }
}
