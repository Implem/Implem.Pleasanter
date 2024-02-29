using Implem.Pleasanter.Models.Shared;
using System;
using System.Collections.Generic;
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
        public string Theme { get; set; }
        public int? FirstAndLastNameOrder { get; set; }
        public string Body { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? PasswordExpirationTime { get; set; }
        public DateTime? PasswordChangeTime { get; set; }
        public int? NumberOfLogins { get; set; }
        public int? NumberOfDenial { get; set; }
        public bool? TenantManager { get; set; }
        public bool? ServiceManager { get; set; }
        public bool? AllowCreationAtTopSite { get; set; }
        public bool? AllowGroupAdministration { get; set; }
        public bool? AllowGroupCreation { get; set; }
        public bool? AllowApi { get; set; }
        public bool? EnableSecondaryAuthentication { get; set; }
        public bool? DisableSecondaryAuthentication { get; set; }
        public bool? Disabled { get; set; }
        public bool? Lockout { get; set; }
        public int? LockoutCounter { get; set; }
        public bool? Developer { get; set; }
        public string UserSettings { get; set; }
        public string PasswordHistries { get; set; }
        public string SecondaryAuthenticationCode { get; set; }
        public DateTime? SecondaryAuthenticationCodeExpirationTime { get; set; }
        public string LdapSearchRoot { get; set; }
        public DateTime? SynchronizedTime { get; set; }
        public string SecretKey { get; set; }
        public bool? EnableSecretKey { get; set; }
        public string Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public List<string> MailAddresses { get; set; }

        public UserApiModel()
        {
        }

        public override object ObjectValue(string columnName)
        {
            switch (columnName)
            {
                case "TenantId": return TenantId;
                case "UserId": return UserId;
                case "Ver": return Ver;
                case "LoginId": return LoginId;
                case "GlobalId": return GlobalId;
                case "Name": return Name;
                case "UserCode": return UserCode;
                case "Password": return Password;
                case "LastName": return LastName;
                case "FirstName": return FirstName;
                case "Birthday": return Birthday;
                case "Gender": return Gender;
                case "Language": return Language;
                case "TimeZone": return TimeZone;
                case "DeptCode": return DeptCode;
                case "DeptId": return DeptId;
                case "Theme": return Theme;
                case "FirstAndLastNameOrder": return FirstAndLastNameOrder;
                case "Body": return Body;
                case "LastLoginTime": return LastLoginTime;
                case "PasswordExpirationTime": return PasswordExpirationTime;
                case "PasswordChangeTime": return PasswordChangeTime;
                case "NumberOfLogins": return NumberOfLogins;
                case "NumberOfDenial": return NumberOfDenial;
                case "TenantManager": return TenantManager;
                case "ServiceManager": return ServiceManager;
                case "AllowCreationAtTopSite": return AllowCreationAtTopSite;
                case "AllowGroupAdministration": return AllowGroupAdministration;
                case "AllowGroupCreation": return AllowGroupCreation;
                case "AllowApi": return AllowApi;
                case "EnableSecondaryAuthentication": return EnableSecondaryAuthentication;
                case "DisableSecondaryAuthentication": return DisableSecondaryAuthentication;
                case "Disabled": return Disabled;
                case "Lockout": return Lockout;
                case "LockoutCounter": return LockoutCounter;
                case "Developer": return Developer;
                case "UserSettings": return UserSettings;
                case "PasswordHistries": return PasswordHistries;
                case "SecondaryAuthenticationCode": return SecondaryAuthenticationCode;
                case "SecondaryAuthenticationCodeExpirationTime": return SecondaryAuthenticationCodeExpirationTime;
                case "LdapSearchRoot": return LdapSearchRoot;
                case "SynchronizedTime": return SynchronizedTime;
                case "SecretKey": return SecretKey;
                case "EnableSecretKey": return EnableSecretKey;
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
