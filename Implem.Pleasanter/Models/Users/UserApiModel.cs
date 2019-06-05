using Implem.Pleasanter.Models.Shared;
using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class UserApiModel : _BaseApiModel
    {
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public int? Ver { get; set; }
        public string LoginId { get; set; }
        public string GlobalId { get; set; }
        public string Name { get; set; }
        public string UserCode { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Gender { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public string DeptCode { get; set; }
        public int? DeptId { get; set; }
        public int? FirstAndLastNameOrder { get; set; }
        public string Body { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? PasswordExpirationTime { get; set; }
        public DateTime? PasswordChangeTime { get; set; }
        public int? NumberOfLogins { get; set; }
        public int? NumberOfDenial { get; set; }
        public bool? TenantManager { get; set; }
        public bool? ServiceManager { get; set; }
        public bool? Disabled { get; set; }
        public bool? Lockout { get; set; }
        public int? LockoutCounter { get; set; }
        public bool? Developer { get; set; }
        public string UserSettings { get; set; }
        public string LdapSearchRoot { get; set; }
        public DateTime? SynchronizedTime { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public UserApiModel()
        {
        }
    }
}
