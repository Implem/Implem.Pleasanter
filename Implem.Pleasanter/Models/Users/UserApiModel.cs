using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class UserApiModel
    {
        public int? TenantId;
        public int? UserId;
        public int? Ver;
        public string LoginId;
        public string GlobalId;
        public string Name;
        public string UserCode;
        public string Password;
        public string LastName;
        public string FirstName;
        public DateTime? Birthday;
        public string Gender;
        public string Language;
        public string TimeZone;
        public int? DeptId;
        public int? FirstAndLastNameOrder;
        public string Body;
        public DateTime? LastLoginTime;
        public DateTime? PasswordExpirationTime;
        public DateTime? PasswordChangeTime;
        public int? NumberOfLogins;
        public int? NumberOfDenial;
        public bool? TenantManager;
        public bool? ServiceManager;
        public bool? Disabled;
        public bool? Developer;
        public string UserSettings;
        public string ApiKey;
        public string Comments;
        public int? Creator;
        public int? Updator;
        public DateTime? CreatedTime;
        public DateTime? UpdatedTime;
        public bool? VerUp;
    }
}
