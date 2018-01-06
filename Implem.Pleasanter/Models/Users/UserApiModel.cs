using System;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class UserApiModel
    {
        public int? UserId;
        public string LoginId;
        public string GlobalId;
        public string Name;
        public string UserCode;
        public string Password;
        public string PasswordValidate;
        public string PasswordDummy;
        public bool? RememberMe;
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
        public bool? Disabled;
        public string ApiKey;
        public string OldPassword;
        public string ChangedPassword;
        public string ChangedPasswordValidator;
        public string AfterResetPassword;
        public string AfterResetPasswordValidator;
        public string DemoMailAddress;
        public string SessionGuid;
        public string Timestamp;
        public string Comments;
        public bool? VerUp;
    }
}
