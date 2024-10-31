using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
using Implem.Pleasanter.Libraries.Extensions;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Models;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.ServerScripts;
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using static Implem.Pleasanter.Libraries.ServerScripts.ServerScriptModel;
namespace Implem.Pleasanter.Models
{
    [Serializable]
    public class UserModel : BaseModel, Interfaces.IConvertable
    {
        public int TenantId = 0;
        public int UserId = 0;
        public string LoginId = string.Empty;
        public string GlobalId = string.Empty;
        public string Name = string.Empty;
        public string UserCode = string.Empty;
        public string Password = string.Empty;
        public string PasswordValidate = string.Empty;
        public string PasswordDummy = string.Empty;
        public bool RememberMe = false;
        public string LastName = string.Empty;
        public string FirstName = string.Empty;
        public Time Birthday = new Time();
        public string Gender = string.Empty;
        public string Language = "en";
        public string TimeZone = "UTC";
        public string DeptCode = string.Empty;
        public int DeptId = 0;
        public string Theme = string.Empty;
        public Names.FirstAndLastNameOrders FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)2;
        public string Body = string.Empty;
        public Time LastLoginTime = new Time();
        public Time PasswordExpirationTime = new Time();
        public Time PasswordChangeTime = new Time();
        public int NumberOfLogins = 0;
        public int NumberOfDenial = 0;
        public bool TenantManager = false;
        public bool ServiceManager = false;
        public bool AllowCreationAtTopSite = false;
        public bool AllowGroupAdministration = false;
        public bool AllowGroupCreation = false;
        public bool AllowApi = false;
        public bool AllowMovingFromTopSite = false;
        public bool EnableSecondaryAuthentication = false;
        public bool DisableSecondaryAuthentication = false;
        public bool Disabled = false;
        public bool Lockout = false;
        public int LockoutCounter = 0;
        public bool Developer = false;
        public UserSettings UserSettings = new UserSettings();
        public string ApiKey = string.Empty;
        public string OldPassword = string.Empty;
        public string ChangedPassword = string.Empty;
        public string ChangedPasswordValidator = string.Empty;
        public string AfterResetPassword = string.Empty;
        public string AfterResetPasswordValidator = string.Empty;
        public List<PasswordHistory> PasswordHistries = new List<PasswordHistory>();
        public List<string> MailAddresses = new List<string>();
        public string DemoMailAddress = string.Empty;
        public string SessionGuid = string.Empty;
        public string SecondaryAuthenticationCode = string.Empty;
        public Time SecondaryAuthenticationCodeExpirationTime = new Time();
        public string LdapSearchRoot = string.Empty;
        public DateTime SynchronizedTime = 0.ToDateTime();
        public string SecretKey = string.Empty;
        public bool EnableSecretKey = false;
        public Time LoginExpirationLimit = new Time();
        public int LoginExpirationPeriod = 0;

        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                return TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(o => o.Id == TimeZone);
            }
        }

        public Dept Dept
        {
            get
            {
                return SiteInfo.Dept(tenantId: TenantId, deptId: DeptId);
            }
        }

        public Title Title
        {
            get
            {
                return new Title(UserId, Name);
            }
        }

        public int SavedTenantId = 0;
        public int SavedUserId = 0;
        public string SavedLoginId = string.Empty;
        public string SavedGlobalId = string.Empty;
        public string SavedName = string.Empty;
        public string SavedUserCode = string.Empty;
        public string SavedPassword = string.Empty;
        public string SavedPasswordValidate = string.Empty;
        public string SavedPasswordDummy = string.Empty;
        public bool SavedRememberMe = false;
        public string SavedLastName = string.Empty;
        public string SavedFirstName = string.Empty;
        public DateTime SavedBirthday = 0.ToDateTime();
        public string SavedGender = string.Empty;
        public string SavedLanguage = "en";
        public string SavedTimeZone = "UTC";
        public string SavedDeptCode = string.Empty;
        public int SavedDeptId = 0;
        public string SavedTheme = string.Empty;
        public int SavedFirstAndLastNameOrder = 2;
        public string SavedBody = string.Empty;
        public DateTime SavedLastLoginTime = 0.ToDateTime();
        public DateTime SavedPasswordExpirationTime = 0.ToDateTime();
        public DateTime SavedPasswordChangeTime = 0.ToDateTime();
        public int SavedNumberOfLogins = 0;
        public int SavedNumberOfDenial = 0;
        public bool SavedTenantManager = false;
        public bool SavedServiceManager = false;
        public bool SavedAllowCreationAtTopSite = false;
        public bool SavedAllowGroupAdministration = false;
        public bool SavedAllowGroupCreation = false;
        public bool SavedAllowApi = false;
        public bool SavedAllowMovingFromTopSite = false;
        public bool SavedEnableSecondaryAuthentication = false;
        public bool SavedDisableSecondaryAuthentication = false;
        public bool SavedDisabled = false;
        public bool SavedLockout = false;
        public int SavedLockoutCounter = 0;
        public bool SavedDeveloper = false;
        public string SavedUserSettings = "{}";
        public string SavedApiKey = string.Empty;
        public string SavedOldPassword = string.Empty;
        public string SavedChangedPassword = string.Empty;
        public string SavedChangedPasswordValidator = string.Empty;
        public string SavedAfterResetPassword = string.Empty;
        public string SavedAfterResetPasswordValidator = string.Empty;
        public string SavedPasswordHistries = "{}";
        public string SavedMailAddresses = string.Empty;
        public string SavedDemoMailAddress = string.Empty;
        public string SavedSessionGuid = string.Empty;
        public string SavedSecondaryAuthenticationCode = string.Empty;
        public DateTime SavedSecondaryAuthenticationCodeExpirationTime = 0.ToDateTime();
        public string SavedLdapSearchRoot = string.Empty;
        public DateTime SavedSynchronizedTime = 0.ToDateTime();
        public string SavedSecretKey = string.Empty;
        public bool SavedEnableSecretKey = false;
        public DateTime SavedLoginExpirationLimit = 0.ToDateTime();
        public int SavedLoginExpirationPeriod = 0;

        public bool TenantId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != TenantId;
            }
            return TenantId != SavedTenantId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool UserId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != UserId;
            }
            return UserId != SavedUserId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != UserId);
        }

        public bool LoginId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != LoginId;
            }
            return LoginId != SavedLoginId && LoginId != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != LoginId);
        }

        public bool GlobalId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != GlobalId;
            }
            return GlobalId != SavedGlobalId && GlobalId != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != GlobalId);
        }

        public bool Name_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Name;
            }
            return Name != SavedName && Name != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Name);
        }

        public bool UserCode_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != UserCode;
            }
            return UserCode != SavedUserCode && UserCode != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != UserCode);
        }

        public bool Password_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Password;
            }
            return Password != SavedPassword && Password != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Password);
        }

        public bool LastName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != LastName;
            }
            return LastName != SavedLastName && LastName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != LastName);
        }

        public bool FirstName_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != FirstName;
            }
            return FirstName != SavedFirstName && FirstName != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != FirstName);
        }

        public bool Gender_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Gender;
            }
            return Gender != SavedGender && Gender != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Gender);
        }

        public bool Language_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Language;
            }
            return Language != SavedLanguage && Language != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Language);
        }

        public bool TimeZone_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != TimeZone;
            }
            return TimeZone != SavedTimeZone && TimeZone != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != TimeZone);
        }

        public bool DeptId_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != DeptId;
            }
            return DeptId != SavedDeptId
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != DeptId);
        }

        public bool Theme_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Theme;
            }
            return Theme != SavedTheme && Theme != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Theme);
        }

        public bool FirstAndLastNameOrder_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != FirstAndLastNameOrder.ToInt();
            }
            return FirstAndLastNameOrder.ToInt() != SavedFirstAndLastNameOrder
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != FirstAndLastNameOrder.ToInt());
        }

        public bool Body_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != Body;
            }
            return Body != SavedBody && Body != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != Body);
        }

        public bool NumberOfLogins_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != NumberOfLogins;
            }
            return NumberOfLogins != SavedNumberOfLogins
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != NumberOfLogins);
        }

        public bool NumberOfDenial_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != NumberOfDenial;
            }
            return NumberOfDenial != SavedNumberOfDenial
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != NumberOfDenial);
        }

        public bool TenantManager_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != TenantManager;
            }
            return TenantManager != SavedTenantManager
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != TenantManager);
        }

        public bool ServiceManager_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != ServiceManager;
            }
            return ServiceManager != SavedServiceManager
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != ServiceManager);
        }

        public bool AllowCreationAtTopSite_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != AllowCreationAtTopSite;
            }
            return AllowCreationAtTopSite != SavedAllowCreationAtTopSite
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != AllowCreationAtTopSite);
        }

        public bool AllowGroupAdministration_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != AllowGroupAdministration;
            }
            return AllowGroupAdministration != SavedAllowGroupAdministration
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != AllowGroupAdministration);
        }

        public bool AllowGroupCreation_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != AllowGroupCreation;
            }
            return AllowGroupCreation != SavedAllowGroupCreation
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != AllowGroupCreation);
        }

        public bool AllowApi_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != AllowApi;
            }
            return AllowApi != SavedAllowApi
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != AllowApi);
        }

        public bool AllowMovingFromTopSite_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != AllowMovingFromTopSite;
            }
            return AllowMovingFromTopSite != SavedAllowMovingFromTopSite
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != AllowMovingFromTopSite);
        }

        public bool EnableSecondaryAuthentication_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != EnableSecondaryAuthentication;
            }
            return EnableSecondaryAuthentication != SavedEnableSecondaryAuthentication
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != EnableSecondaryAuthentication);
        }

        public bool DisableSecondaryAuthentication_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != DisableSecondaryAuthentication;
            }
            return DisableSecondaryAuthentication != SavedDisableSecondaryAuthentication
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != DisableSecondaryAuthentication);
        }

        public bool Disabled_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != Disabled;
            }
            return Disabled != SavedDisabled
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != Disabled);
        }

        public bool Lockout_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != Lockout;
            }
            return Lockout != SavedLockout
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != Lockout);
        }

        public bool LockoutCounter_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != LockoutCounter;
            }
            return LockoutCounter != SavedLockoutCounter
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != LockoutCounter);
        }

        public bool Developer_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != Developer;
            }
            return Developer != SavedDeveloper
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != Developer);
        }

        public bool UserSettings_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != UserSettings.RecordingJson();
            }
            return UserSettings.RecordingJson() != SavedUserSettings && UserSettings.RecordingJson() != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != UserSettings.RecordingJson());
        }

        public bool ApiKey_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != ApiKey;
            }
            return ApiKey != SavedApiKey && ApiKey != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != ApiKey);
        }

        public bool PasswordHistries_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != PasswordHistries.ToJson();
            }
            return PasswordHistries.ToJson() != SavedPasswordHistries && PasswordHistries.ToJson() != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != PasswordHistries.ToJson());
        }

        public bool SecondaryAuthenticationCode_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != SecondaryAuthenticationCode;
            }
            return SecondaryAuthenticationCode != SavedSecondaryAuthenticationCode && SecondaryAuthenticationCode != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != SecondaryAuthenticationCode);
        }

        public bool LdapSearchRoot_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != LdapSearchRoot;
            }
            return LdapSearchRoot != SavedLdapSearchRoot && LdapSearchRoot != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != LdapSearchRoot);
        }

        public bool SecretKey_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToString() != SecretKey;
            }
            return SecretKey != SavedSecretKey && SecretKey != null
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToString() != SecretKey);
        }

        public bool EnableSecretKey_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToBool() != EnableSecretKey;
            }
            return EnableSecretKey != SavedEnableSecretKey
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToBool() != EnableSecretKey);
        }

        public bool LoginExpirationPeriod_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToInt() != LoginExpirationPeriod;
            }
            return LoginExpirationPeriod != SavedLoginExpirationPeriod
                &&  (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.GetDefaultInput(context: context).ToInt() != LoginExpirationPeriod);
        }

        public bool Birthday_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != Birthday.Value;
            }
            return Birthday.Value != SavedBirthday
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != Birthday.Value.Date);
        }

        public bool LastLoginTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != LastLoginTime.Value;
            }
            return LastLoginTime.Value != SavedLastLoginTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != LastLoginTime.Value.Date);
        }

        public bool PasswordExpirationTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != PasswordExpirationTime.Value;
            }
            return PasswordExpirationTime.Value != SavedPasswordExpirationTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != PasswordExpirationTime.Value.Date);
        }

        public bool PasswordChangeTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != PasswordChangeTime.Value;
            }
            return PasswordChangeTime.Value != SavedPasswordChangeTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != PasswordChangeTime.Value.Date);
        }

        public bool SecondaryAuthenticationCodeExpirationTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != SecondaryAuthenticationCodeExpirationTime.Value;
            }
            return SecondaryAuthenticationCodeExpirationTime.Value != SavedSecondaryAuthenticationCodeExpirationTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != SecondaryAuthenticationCodeExpirationTime.Value.Date);
        }

        public bool SynchronizedTime_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != SynchronizedTime;
            }
            return SynchronizedTime != SavedSynchronizedTime
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != SynchronizedTime.Date);
        }

        public bool LoginExpirationLimit_Updated(Context context, bool copy = false, Column column = null)
        {
            if (copy && column?.CopyByDefault == true)
            {
                return column.GetDefaultInput(context: context).ToDateTime() != LoginExpirationLimit.Value;
            }
            return LoginExpirationLimit.Value != SavedLoginExpirationLimit
                && (column == null
                    || column.DefaultInput.IsNullOrEmpty()
                    || column.DefaultTime(context: context).Date != LoginExpirationLimit.Value.Date);
        }

        public UserSettings Session_UserSettings(Context context)
        {
            return context.SessionData.Get("UserSettings") != null
                ? context.SessionData.Get("UserSettings")?.ToString().Deserialize<UserSettings>() ?? new UserSettings()
                : UserSettings;
        }

        public void Session_UserSettings(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "UserSettings",
                value: value,
                page: true);
        }

        public List<string> Session_MailAddresses(Context context)
        {
            return context.SessionData.Get("MailAddresses") != null
                ? context.SessionData.Get("MailAddresses").Deserialize<List<string>>() ?? new List<string>()
                : MailAddresses;
        }

        public void Session_MailAddresses(Context context, string value)
        {
            SessionUtilities.Set(
                context: context,
                key: "MailAddresses",
                value: value,
                page: true);
        }

        public string CsvData(
            Context context,
            SiteSettings ss,
            Column column,
            ExportColumn exportColumn,
            List<string> mine,
            bool? encloseDoubleQuotes)
        {
            var value = string.Empty;
            switch (column.Name)
            {
                case "TenantId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? TenantId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UserId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UserId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Ver":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Ver.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "LoginId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? LoginId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "GlobalId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? GlobalId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Name":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Name.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UserCode":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UserCode.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Password":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Password.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "LastName":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? LastName.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "FirstName":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? FirstName.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Birthday":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Birthday.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Gender":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Gender.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Language":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Language.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "TimeZone":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? TimeZone.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "TimeZoneInfo":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? TimeZoneInfo.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DeptCode":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? DeptCode.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DeptId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? DeptId.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Dept":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Dept.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Theme":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Theme.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "FirstAndLastNameOrder":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? FirstAndLastNameOrder.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Body":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Body.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "LastLoginTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? LastLoginTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "PasswordExpirationTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? PasswordExpirationTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "PasswordChangeTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? PasswordChangeTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumberOfLogins":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? NumberOfLogins.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "NumberOfDenial":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? NumberOfDenial.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "TenantManager":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? TenantManager.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ServiceManager":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ServiceManager.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AllowCreationAtTopSite":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? AllowCreationAtTopSite.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AllowGroupAdministration":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? AllowGroupAdministration.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AllowGroupCreation":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? AllowGroupCreation.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AllowApi":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? AllowApi.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "AllowMovingFromTopSite":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? AllowMovingFromTopSite.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "EnableSecondaryAuthentication":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? EnableSecondaryAuthentication.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "DisableSecondaryAuthentication":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? DisableSecondaryAuthentication.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Disabled":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Disabled.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Lockout":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Lockout.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "LockoutCounter":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? LockoutCounter.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Developer":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Developer.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UserSettings":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UserSettings.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "ApiKey":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? ApiKey.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "SecondaryAuthenticationCode":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SecondaryAuthenticationCode.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "SecondaryAuthenticationCodeExpirationTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SecondaryAuthenticationCodeExpirationTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "LdapSearchRoot":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? LdapSearchRoot.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "SynchronizedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SynchronizedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "SecretKey":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? SecretKey.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "EnableSecretKey":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? EnableSecretKey.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "LoginExpirationLimit":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? LoginExpirationLimit.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "LoginExpirationPeriod":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? LoginExpirationPeriod.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Comments":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Comments.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Creator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Creator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "Updator":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? Updator.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "CreatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? CreatedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                case "UpdatedTime":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: mine)
                            ? UpdatedTime?.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                                    ?? String.Empty
                            : string.Empty;
                    break;
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetClass(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Num":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetNum(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Date":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetDate(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Description":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetDescription(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Check":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetCheck(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        case "Attachments":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                mine: mine)
                                    ? GetAttachments(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        default: return string.Empty;
                    }
                    break;
            }
            return CsvUtilities.EncloseDoubleQuotes(
                value: value,
                encloseDoubleQuotes: encloseDoubleQuotes);
        }

        public List<int> SwitchTargets;

        /// <summary>
        /// Fixed:
        /// </summary>
        public UserModel()
        {
            InitializeTimeZone();
        }

        public UserModel(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData = null,
            UserApiModel userApiModel = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            SetTimeZoneAndLanguage(
                            context: context,
                            ss: ss,
                            methodType: methodType);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (userApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: userApiModel);
            }
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public UserModel(
            Context context,
            SiteSettings ss,
            int userId,
            Dictionary<string, string> formData = null,
            UserApiModel userApiModel = null,
            SqlColumnCollection column = null,
            bool clearSessions = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            UserId = userId;
            SetTimeZoneAndLanguage(
                            context: context,
                            ss: ss,
                            methodType: methodType);
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(
                    context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    column: column,
                    where: Rds.UsersWhereDefault(
                        context: context,
                        userModel: this)
                            .Users_Ver(context.QueryStrings.Int("ver")), ss: ss);
            }
            else
            {
                Get(
                    context: context,
                    ss: ss,
                    column: column);
            }
            if (clearSessions) ClearSessions(context: context);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (userApiModel != null)
            {
                SetByApi(context: context, ss: ss, data: userApiModel);
            }
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public UserModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            Dictionary<string, string> formData = null,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            TenantId = context.TenantId;
            if (dataRow != null)
            {
                Set(
                    context: context,
                    ss: ss,
                    dataRow: dataRow,
                    tableAlias: tableAlias);
            }
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            OnConstructed(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnConstructing(Context context)
        {
            InitializeTimeZone();
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
            Session_UserSettings(context: context, value: null);
            Session_MailAddresses(context: context, value: null);
        }

        public UserModel Get(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            where = where ?? Rds.UsersWhereDefault(
                context: context,
                userModel: this);
            column = (column ?? Rds.UsersDefaultColumns());
            join = join ?? Rds.UsersJoinDefault();
            Set(context, ss, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectUsers(
                    tableType: tableType,
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public UserApiModel GetByApi(Context context, SiteSettings ss, bool? getMailAddresses)
        {
            var data = new UserApiModel()
            {
                ApiVersion = context.ApiVersion
            };
            ss.ReadableColumns(context: context, noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "TenantId": data.TenantId = TenantId; break;
                    case "UserId": data.UserId = UserId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "LoginId": data.LoginId = LoginId; break;
                    case "GlobalId": data.GlobalId = GlobalId; break;
                    case "Name": data.Name = Name; break;
                    case "UserCode": data.UserCode = UserCode; break;
                    case "Password": data.Password = Password; break;
                    case "LastName": data.LastName = LastName; break;
                    case "FirstName": data.FirstName = FirstName; break;
                    case "Birthday": data.Birthday = Birthday.Value.ToLocal(context: context); break;
                    case "Gender": data.Gender = Gender; break;
                    case "Language": data.Language = Language; break;
                    case "TimeZone": data.TimeZone = TimeZone; break;
                    case "DeptCode": data.DeptCode = DeptCode; break;
                    case "DeptId": data.DeptId = DeptId; break;
                    case "Theme": data.Theme = Theme; break;
                    case "FirstAndLastNameOrder": data.FirstAndLastNameOrder = FirstAndLastNameOrder.ToInt(); break;
                    case "Body": data.Body = Body; break;
                    case "LastLoginTime": data.LastLoginTime = LastLoginTime.Value.ToLocal(context: context); break;
                    case "PasswordExpirationTime": data.PasswordExpirationTime = PasswordExpirationTime.Value.ToLocal(context: context); break;
                    case "PasswordChangeTime": data.PasswordChangeTime = PasswordChangeTime.Value.ToLocal(context: context); break;
                    case "NumberOfLogins": data.NumberOfLogins = NumberOfLogins; break;
                    case "NumberOfDenial": data.NumberOfDenial = NumberOfDenial; break;
                    case "TenantManager": data.TenantManager = TenantManager; break;
                    case "ServiceManager": data.ServiceManager = ServiceManager; break;
                    case "AllowCreationAtTopSite": data.AllowCreationAtTopSite = AllowCreationAtTopSite; break;
                    case "AllowGroupAdministration": data.AllowGroupAdministration = AllowGroupAdministration; break;
                    case "AllowGroupCreation": data.AllowGroupCreation = AllowGroupCreation; break;
                    case "AllowApi": data.AllowApi = AllowApi; break;
                    case "AllowMovingFromTopSite": data.AllowMovingFromTopSite = AllowMovingFromTopSite; break;
                    case "EnableSecondaryAuthentication": data.EnableSecondaryAuthentication = EnableSecondaryAuthentication; break;
                    case "DisableSecondaryAuthentication": data.DisableSecondaryAuthentication = DisableSecondaryAuthentication; break;
                    case "Disabled": data.Disabled = Disabled; break;
                    case "Lockout": data.Lockout = Lockout; break;
                    case "LockoutCounter": data.LockoutCounter = LockoutCounter; break;
                    case "Developer": data.Developer = Developer; break;
                    case "UserSettings": data.UserSettings = UserSettings.RecordingJson(); break;
                    case "PasswordHistries": data.PasswordHistries = PasswordHistries.ToJson(); break;
                    case "SecondaryAuthenticationCode": data.SecondaryAuthenticationCode = SecondaryAuthenticationCode; break;
                    case "SecondaryAuthenticationCodeExpirationTime": data.SecondaryAuthenticationCodeExpirationTime = SecondaryAuthenticationCodeExpirationTime.Value.ToLocal(context: context); break;
                    case "LdapSearchRoot": data.LdapSearchRoot = LdapSearchRoot; break;
                    case "SynchronizedTime": data.SynchronizedTime = SynchronizedTime.ToLocal(context: context); break;
                    case "SecretKey": data.SecretKey = SecretKey; break;
                    case "EnableSecretKey": data.EnableSecretKey = EnableSecretKey; break;
                    case "LoginExpirationLimit": data.LoginExpirationLimit = LoginExpirationLimit.Value.ToLocal(context: context); break;
                    case "LoginExpirationPeriod": data.LoginExpirationPeriod = LoginExpirationPeriod; break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                    default: 
                        data.Value(
                            context: context,
                            column: column,
                            value: GetValue(
                                context: context,
                                column: column,
                                toLocal: true));
                        break;
                }
            });
            if (getMailAddresses == true)
            {
                data.MailAddresses = GetMailAddresses(
                    context: context,
                    ss: ss,
                    userId: data.UserId);
            }
            return data;
        }

        public string ToDisplay(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "UserId":
                    return UserId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginId":
                    return LoginId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "GlobalId":
                    return GlobalId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Name":
                    return Name.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserCode":
                    return UserCode.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Password":
                    return Password.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordValidate":
                    return PasswordValidate.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordDummy":
                    return PasswordDummy.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "RememberMe":
                    return RememberMe.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "LastName":
                    return LastName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "FirstName":
                    return FirstName.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Birthday":
                    return Birthday.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Gender":
                    return Gender.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Language":
                    return Language.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "TimeZone":
                    return TimeZone.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "DeptCode":
                    return DeptCode.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "DeptId":
                    return DeptId.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Theme":
                    return Theme.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Body":
                    return Body.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "LastLoginTime":
                    return LastLoginTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordExpirationTime":
                    return PasswordExpirationTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordChangeTime":
                    return PasswordChangeTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "NumberOfLogins":
                    return NumberOfLogins.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "NumberOfDenial":
                    return NumberOfDenial.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "TenantManager":
                    return TenantManager.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowCreationAtTopSite":
                    return AllowCreationAtTopSite.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowGroupAdministration":
                    return AllowGroupAdministration.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowGroupCreation":
                    return AllowGroupCreation.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowApi":
                    return AllowApi.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowMovingFromTopSite":
                    return AllowMovingFromTopSite.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "EnableSecondaryAuthentication":
                    return EnableSecondaryAuthentication.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "DisableSecondaryAuthentication":
                    return DisableSecondaryAuthentication.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Disabled":
                    return Disabled.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Lockout":
                    return Lockout.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "LockoutCounter":
                    return LockoutCounter.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ApiKey":
                    return ApiKey.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "OldPassword":
                    return OldPassword.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ChangedPassword":
                    return ChangedPassword.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "ChangedPasswordValidator":
                    return ChangedPasswordValidator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "AfterResetPassword":
                    return AfterResetPassword.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "AfterResetPasswordValidator":
                    return AfterResetPasswordValidator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "DemoMailAddress":
                    return DemoMailAddress.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionGuid":
                    return SessionGuid.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SecondaryAuthenticationCode":
                    return SecondaryAuthenticationCode.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SecondaryAuthenticationCodeExpirationTime":
                    return SecondaryAuthenticationCodeExpirationTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapSearchRoot":
                    return LdapSearchRoot.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SynchronizedTime":
                    return SynchronizedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "SecretKey":
                    return SecretKey.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "EnableSecretKey":
                    return EnableSecretKey.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginExpirationLimit":
                    return LoginExpirationLimit.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginExpirationPeriod":
                    return LoginExpirationPeriod.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Dept":
                    return Dept.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToDisplay(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "TenantId":
                    return TenantId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserId":
                    return UserId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginId":
                    return LoginId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "GlobalId":
                    return GlobalId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Name":
                    return Name.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserCode":
                    return UserCode.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Password":
                    return Password.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordValidate":
                    return PasswordValidate.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordDummy":
                    return PasswordDummy.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "RememberMe":
                    return RememberMe.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LastName":
                    return LastName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "FirstName":
                    return FirstName.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Birthday":
                    return Birthday.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Gender":
                    return Gender.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Language":
                    return Language.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TimeZone":
                    return TimeZone.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TimeZoneInfo":
                    return TimeZoneInfo.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "DeptCode":
                    return DeptCode.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "DeptId":
                    return DeptId.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Dept":
                    return Dept.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Theme":
                    return Theme.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Body":
                    return Body.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LastLoginTime":
                    return LastLoginTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordExpirationTime":
                    return PasswordExpirationTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordChangeTime":
                    return PasswordChangeTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "NumberOfLogins":
                    return NumberOfLogins.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "NumberOfDenial":
                    return NumberOfDenial.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TenantManager":
                    return TenantManager.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ServiceManager":
                    return ServiceManager.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowCreationAtTopSite":
                    return AllowCreationAtTopSite.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowGroupAdministration":
                    return AllowGroupAdministration.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowGroupCreation":
                    return AllowGroupCreation.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowApi":
                    return AllowApi.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowMovingFromTopSite":
                    return AllowMovingFromTopSite.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "EnableSecondaryAuthentication":
                    return EnableSecondaryAuthentication.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "DisableSecondaryAuthentication":
                    return DisableSecondaryAuthentication.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Disabled":
                    return Disabled.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Lockout":
                    return Lockout.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LockoutCounter":
                    return LockoutCounter.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Developer":
                    return Developer.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ApiKey":
                    return ApiKey.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "OldPassword":
                    return OldPassword.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ChangedPassword":
                    return ChangedPassword.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ChangedPasswordValidator":
                    return ChangedPasswordValidator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AfterResetPassword":
                    return AfterResetPassword.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AfterResetPasswordValidator":
                    return AfterResetPasswordValidator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "DemoMailAddress":
                    return DemoMailAddress.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionGuid":
                    return SessionGuid.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SecondaryAuthenticationCode":
                    return SecondaryAuthenticationCode.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SecondaryAuthenticationCodeExpirationTime":
                    return SecondaryAuthenticationCodeExpirationTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapSearchRoot":
                    return LdapSearchRoot.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SynchronizedTime":
                    return SynchronizedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SecretKey":
                    return SecretKey.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "EnableSecretKey":
                    return EnableSecretKey.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginExpirationLimit":
                    return LoginExpirationLimit.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginExpirationPeriod":
                    return LoginExpirationPeriod.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VerUp":
                    return VerUp.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToApiDisplayValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "MailAddresses":
                    return GetMailAddresses(
                        context: context,
                        ss: ss,
                        userId: UserId);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToApiDisplayValue(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public object ToApiValue(Context context, SiteSettings ss, Column column, List<string> mine)
        {
            if (!ss.ReadColumnAccessControls.Allowed(
                context: context,
                ss: ss,
                column: column,
                mine: mine))
            {
                return string.Empty;
            }
            switch (column.Name)
            {
                case "TenantId":
                    return TenantId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserId":
                    return UserId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Ver":
                    return Ver.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginId":
                    return LoginId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "GlobalId":
                    return GlobalId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Name":
                    return Name.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UserCode":
                    return UserCode.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Password":
                    return Password.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordValidate":
                    return PasswordValidate.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordDummy":
                    return PasswordDummy.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "RememberMe":
                    return RememberMe.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LastName":
                    return LastName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "FirstName":
                    return FirstName.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Birthday":
                    return Birthday.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Gender":
                    return Gender.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Language":
                    return Language.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TimeZone":
                    return TimeZone.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TimeZoneInfo":
                    return TimeZoneInfo.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "DeptCode":
                    return DeptCode.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "DeptId":
                    return DeptId.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Dept":
                    return Dept.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Theme":
                    return Theme.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Title":
                    return Title.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Body":
                    return Body.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LastLoginTime":
                    return LastLoginTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordExpirationTime":
                    return PasswordExpirationTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "PasswordChangeTime":
                    return PasswordChangeTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "NumberOfLogins":
                    return NumberOfLogins.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "NumberOfDenial":
                    return NumberOfDenial.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "TenantManager":
                    return TenantManager.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ServiceManager":
                    return ServiceManager.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowCreationAtTopSite":
                    return AllowCreationAtTopSite.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowGroupAdministration":
                    return AllowGroupAdministration.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowGroupCreation":
                    return AllowGroupCreation.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowApi":
                    return AllowApi.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AllowMovingFromTopSite":
                    return AllowMovingFromTopSite.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "EnableSecondaryAuthentication":
                    return EnableSecondaryAuthentication.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "DisableSecondaryAuthentication":
                    return DisableSecondaryAuthentication.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Disabled":
                    return Disabled.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Lockout":
                    return Lockout.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LockoutCounter":
                    return LockoutCounter.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Developer":
                    return Developer.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ApiKey":
                    return ApiKey.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "OldPassword":
                    return OldPassword.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ChangedPassword":
                    return ChangedPassword.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "ChangedPasswordValidator":
                    return ChangedPasswordValidator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AfterResetPassword":
                    return AfterResetPassword.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "AfterResetPasswordValidator":
                    return AfterResetPasswordValidator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "DemoMailAddress":
                    return DemoMailAddress.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SessionGuid":
                    return SessionGuid.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SecondaryAuthenticationCode":
                    return SecondaryAuthenticationCode.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SecondaryAuthenticationCodeExpirationTime":
                    return SecondaryAuthenticationCodeExpirationTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LdapSearchRoot":
                    return LdapSearchRoot.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SynchronizedTime":
                    return SynchronizedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "SecretKey":
                    return SecretKey.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "EnableSecretKey":
                    return EnableSecretKey.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginExpirationLimit":
                    return LoginExpirationLimit.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "LoginExpirationPeriod":
                    return LoginExpirationPeriod.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Comments":
                    return Comments.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Creator":
                    return Creator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Updator":
                    return Updator.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "CreatedTime":
                    return CreatedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "UpdatedTime":
                    return UpdatedTime.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "VerUp":
                    return VerUp.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                case "Timestamp":
                    return Timestamp.ToApiValue(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                    {
                        case "Class":
                            return GetClass(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Num":
                            return GetNum(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Date":
                            return GetDate(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Description":
                            return GetDescription(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Check":
                            return GetCheck(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        case "Attachments":
                            return GetAttachments(columnName: column.Name).ToApiValue(
                                context: context,
                                ss: ss,
                                column: column);
                        default:
                            return string.Empty;
                    }
            }
        }

        public ErrorData Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            string noticeType = "Created",
            bool otherInitValue = false,
            bool get = true)
        {
            var userApiModel = context.RequestDataString.Deserialize<UserApiModel>();
            if (userApiModel != null &&
                userApiModel.MailAddresses != null)
            {
                var errorData = UserValidators.OnApiUpdatingMailAddress(userApiModel: userApiModel);
                if (errorData.Type != Error.Types.None)
                {
                    return errorData;
                }
            }
            TenantId = context.TenantId;
            if (Parameters.Security.EnforcePasswordHistories > 0)
            {
                SetPasswordHistories(
                    context: context,
                    password: Password);
            }
            if (!PasswordExpirationTime.Value.InRange())
            {
                SetPasswordExpirationPeriod(context: context);
            }
            var statements = new List<SqlStatement>();
            statements.AddRange(CreateStatements(
                context: context,
                ss: ss,
                tableType: tableType,
                param: param,
                otherInitValue: otherInitValue));
            try
            {
                var response = Repository.ExecuteScalar_response(
                    context: context,
                    transactional: true,
                    selectIdentity: true,
                    statements: statements.ToArray());
                UserId = response.Id.ToInt();
            }
            catch (DbException e)
            {
                if (context.SqlErrors.ErrorCode(e) == context.SqlErrors.ErrorCodeDuplicateKey)
                {
                    return new ErrorData(type: Error.Types.LoginIdAlreadyUse);
                }
                else
                {
                    throw;
                }
            }
            if (get) Get(context: context, ss: ss);
            if (userApiModel != null)
            {
                if (userApiModel.MailAddresses != null)
                {
                    UpdateMailAddresses(context: context, userApiModel: userApiModel);
                }
            }
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            var statements = new List<SqlStatement>();
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertUsers(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.UsersParamDefault(
                        context: context,
                        ss: ss,
                        userModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.UsersUpdated),
            });
            return statements;
        }

        public ErrorData Update(
            Context context,
            SiteSettings ss,
            bool updateMailAddresses = true,
            bool refleshSiteInfo = true,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true,
            bool checkConflict = true)
        {
            var userApiModel = context.RequestDataString.Deserialize<UserApiModel>();
            if (updateMailAddresses &&
                userApiModel != null &&
                userApiModel.MailAddresses != null)
            {
                var errorData = UserValidators.OnApiUpdatingMailAddress(userApiModel: userApiModel);
                if (errorData.Type != Error.Types.None)
                {
                    return errorData;
                }
            }
            if (setBySession)
            {
                SetBySession(context: context);
            }
            var statements = new List<SqlStatement>();
            var verUp = Versions.VerUp(
                context: context,
                ss: ss,
                verUp: VerUp);
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements,
                checkConflict: checkConflict,
                verUp: verUp));
            try
            {
                var response = Repository.ExecuteScalar_response(
                    context: context,
                    transactional: true,
                    statements: statements.ToArray());
                if (response.Event == "Conflicted")
                {
                    return new ErrorData(
                        type: Error.Types.UpdateConflicts,
                        id: UserId);
                }
            }
            catch (DbException e)
            {
                if (context.SqlErrors.ErrorCode(e) == context.SqlErrors.ErrorCodeDuplicateKey)
                {
                    return new ErrorData(type: Error.Types.LoginIdAlreadyUse);
                }
                else
                {
                    throw;
                }
            }
            if (get)
            {
                Get(context: context, ss: ss);
            }
            if (updateMailAddresses)
            {
                if (userApiModel != null)
                {
                    if (userApiModel.MailAddresses != null)
                    {
                        UpdateMailAddresses(context: context, userApiModel: userApiModel);
                    }
                }
                else
                {
                    UpdateMailAddresses(context: context);
                }
            }
            if (refleshSiteInfo)
            {
                SiteInfo.Reflesh(context: context);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null,
            bool checkConflict = true,
            bool verUp = false)
        {
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.UsersWhereDefault(
                context: context,
                userModel: this)
                    .UpdatedTime(timestamp, _using: timestamp.InRange() && checkConflict);
            if (verUp)
            {
                statements.Add(Rds.UsersCopyToStatement(
                    where: where,
                    tableType: Sqls.TableTypes.History,
                    ColumnNames()));
                Ver++;
            }
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                dataTableName: dataTableName,
                where: where,
                param: param,
                otherInitValue: otherInitValue));
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            return new List<SqlStatement>
            {
                Rds.UpdateUsers(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.UsersParamDefault(
                        context: context,
                        ss: ss,
                        userModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement()
                {
                    DataTableName = dataTableName,
                    IfConflicted = true,
                    Id = UserId
                },
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.UsersUpdated),
            };
        }

        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.UsersWhere().UserId(UserId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteBinaries(
                    factory: context,
                    where: Rds.BinariesWhere()
                        .TenantId(context.TenantId)
                        .ReferenceId(UserId)
                        .BinaryType(value: "TenantManagementImages")),
                Rds.DeleteUsers(
                    factory: context,
                    where: where),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.UsersUpdated),
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            var userHash = SiteInfo.TenantCaches.Get(context.TenantId)?.UserHash;
            if (userHash.Keys.Contains(UserId))
            {
                userHash.Remove(UserId);
            }
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, SiteSettings ss,int userId)
        {
            UserId = userId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreUsers(
                        factory: context,
                        where: Rds.UsersWhere().UserId(UserId)),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.UsersUpdated),
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteUsers(
                    tableType: tableType,
                    where: Rds.UsersWhere().UserId(UserId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByForm(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData)
        {
            SetByFormData(
                context: context,
                ss: ss,
                formData: formData);
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Ver = context.QueryStrings.Int("ver");
            }
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = formData.Get("ControlId")?
                    .Split(',')
                    ._2nd()
                    .ToInt() ?? 0;
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
        }

        private void SetByFormData(Context context, SiteSettings ss, Dictionary<string, string> formData)
        {
            formData.ForEach(data =>
            {
                var key = data.Key;
                var value = data.Value ?? string.Empty;
                switch (key)
                {
                    case "Users_LoginId": LoginId = value.ToString(); break;
                    case "Users_GlobalId": GlobalId = value.ToString(); break;
                    case "Users_Name": Name = value.ToString(); break;
                    case "Users_UserCode": UserCode = value.ToString(); break;
                    case "Users_Password": Password = value.ToString().Sha512Cng(); break;
                    case "Users_PasswordValidate": PasswordValidate = value.ToString().Sha512Cng(); break;
                    case "Users_PasswordDummy": PasswordDummy = value.ToString().Sha512Cng(); break;
                    case "Users_RememberMe": RememberMe = value.ToBool(); break;
                    case "Users_LastName": LastName = value.ToString(); break;
                    case "Users_FirstName": FirstName = value.ToString(); break;
                    case "Users_Birthday": Birthday = new Time(context, value.ToDateTime(), byForm: true); break;
                    case "Users_Gender": Gender = value.ToString(); break;
                    case "Users_Language": Language = value.ToString(); break;
                    case "Users_TimeZone": TimeZone = value.ToString(); break;
                    case "Users_DeptCode": DeptCode = value.ToString(); break;
                    case "Users_DeptId": DeptId = value.ToInt(); break;
                    case "Users_Theme": Theme = value.ToString(); break;
                    case "Users_FirstAndLastNameOrder": FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)value.ToInt(); break;
                    case "Users_Body": Body = value.ToString(); break;
                    case "Users_LastLoginTime": LastLoginTime = new Time(context, value.ToDateTime(), byForm: true); break;
                    case "Users_PasswordExpirationTime": PasswordExpirationTime = new Time(context, value.ToDateTime(), byForm: true); break;
                    case "Users_PasswordChangeTime": PasswordChangeTime = new Time(context, value.ToDateTime(), byForm: true); break;
                    case "Users_NumberOfLogins": NumberOfLogins = value.ToInt(); break;
                    case "Users_NumberOfDenial": NumberOfDenial = value.ToInt(); break;
                    case "Users_TenantManager": TenantManager = value.ToBool(); break;
                    case "Users_AllowCreationAtTopSite": AllowCreationAtTopSite = value.ToBool(); break;
                    case "Users_AllowGroupAdministration": AllowGroupAdministration = value.ToBool(); break;
                    case "Users_AllowGroupCreation": AllowGroupCreation = value.ToBool(); break;
                    case "Users_AllowApi": AllowApi = value.ToBool(); break;
                    case "Users_AllowMovingFromTopSite": AllowMovingFromTopSite = value.ToBool(); break;
                    case "Users_EnableSecondaryAuthentication": EnableSecondaryAuthentication = value.ToBool(); break;
                    case "Users_DisableSecondaryAuthentication": DisableSecondaryAuthentication = value.ToBool(); break;
                    case "Users_Disabled": Disabled = value.ToBool(); break;
                    case "Users_Lockout": Lockout = value.ToBool(); if (Lockout_Updated(context: context) && !Lockout) LockoutCounter = 0; break;
                    case "Users_LockoutCounter": LockoutCounter = value.ToInt(); break;
                    case "Users_ApiKey": ApiKey = value.ToString(); break;
                    case "Users_OldPassword": OldPassword = value.ToString().Sha512Cng(); break;
                    case "Users_ChangedPassword": ChangedPassword = value.ToString().Sha512Cng(); break;
                    case "Users_ChangedPasswordValidator": ChangedPasswordValidator = value.ToString().Sha512Cng(); break;
                    case "Users_AfterResetPassword": AfterResetPassword = value.ToString().Sha512Cng(); break;
                    case "Users_AfterResetPasswordValidator": AfterResetPasswordValidator = value.ToString().Sha512Cng(); break;
                    case "Users_DemoMailAddress": DemoMailAddress = value.ToString(); break;
                    case "Users_SessionGuid": SessionGuid = value.ToString(); break;
                    case "Users_SecondaryAuthenticationCode": SecondaryAuthenticationCode = value.ToString().Sha512Cng(); break;
                    case "Users_SecondaryAuthenticationCodeExpirationTime": SecondaryAuthenticationCodeExpirationTime = new Time(context, value.ToDateTime(), byForm: true); break;
                    case "Users_LdapSearchRoot": LdapSearchRoot = value.ToString(); break;
                    case "Users_SynchronizedTime": SynchronizedTime = value.ToDateTime().ToUniversal(context: context); break;
                    case "Users_SecretKey": SecretKey = value.ToString(); break;
                    case "Users_EnableSecretKey": EnableSecretKey = value.ToBool(); break;
                    case "Users_LoginExpirationLimit": LoginExpirationLimit = new Time(context, value.ToDateTime(), byForm: true); break;
                    case "Users_LoginExpirationPeriod": LoginExpirationPeriod = value.ToInt(); break;
                    case "Users_Timestamp": Timestamp = value.ToString(); break;
                    case "Comments": Comments.Prepend(
                        context: context,
                        ss: ss,
                        body: value); break;
                    case "VerUp": VerUp = value.ToBool(); break;
                    default:
                        if (key.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: key.Substring("Comment".Length).ToInt(),
                                body: value);
                        }
                        else
                        {
                            var column = ss.GetColumn(
                                context: context,
                                columnName: key.Split_2nd('_'));
                            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName ?? string.Empty))
                            {
                                case "Class":
                                    SetClass(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Num":
                                    SetNum(
                                        columnName: column.ColumnName,
                                        value: new Num(
                                            context: context,
                                            column: column,
                                            value: value));
                                    break;
                                case "Date":
                                    SetDate(
                                        columnName: column.ColumnName,
                                        value: value.ToDateTime().ToUniversal(context: context));
                                    break;
                                case "Description":
                                    SetDescription(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Check":
                                    SetCheck(
                                        columnName: column.ColumnName,
                                        value: value.ToBool());
                                    break;
                                case "Attachments":
                                    SetAttachments(
                                        columnName: column.ColumnName,
                                        value: value.Deserialize<Attachments>());
                                    break;
                            }
                        }
                        break;
                }
            });
        }

        public void SetByModel(UserModel userModel)
        {
            TenantId = userModel.TenantId;
            LoginId = userModel.LoginId;
            GlobalId = userModel.GlobalId;
            Name = userModel.Name;
            UserCode = userModel.UserCode;
            Password = userModel.Password;
            PasswordValidate = userModel.PasswordValidate;
            PasswordDummy = userModel.PasswordDummy;
            RememberMe = userModel.RememberMe;
            LastName = userModel.LastName;
            FirstName = userModel.FirstName;
            Birthday = userModel.Birthday;
            Gender = userModel.Gender;
            Language = userModel.Language;
            TimeZone = userModel.TimeZone;
            DeptCode = userModel.DeptCode;
            DeptId = userModel.DeptId;
            Theme = userModel.Theme;
            FirstAndLastNameOrder = userModel.FirstAndLastNameOrder;
            Body = userModel.Body;
            LastLoginTime = userModel.LastLoginTime;
            PasswordExpirationTime = userModel.PasswordExpirationTime;
            PasswordChangeTime = userModel.PasswordChangeTime;
            NumberOfLogins = userModel.NumberOfLogins;
            NumberOfDenial = userModel.NumberOfDenial;
            TenantManager = userModel.TenantManager;
            ServiceManager = userModel.ServiceManager;
            AllowCreationAtTopSite = userModel.AllowCreationAtTopSite;
            AllowGroupAdministration = userModel.AllowGroupAdministration;
            AllowGroupCreation = userModel.AllowGroupCreation;
            AllowApi = userModel.AllowApi;
            AllowMovingFromTopSite = userModel.AllowMovingFromTopSite;
            EnableSecondaryAuthentication = userModel.EnableSecondaryAuthentication;
            DisableSecondaryAuthentication = userModel.DisableSecondaryAuthentication;
            Disabled = userModel.Disabled;
            Lockout = userModel.Lockout;
            LockoutCounter = userModel.LockoutCounter;
            Developer = userModel.Developer;
            UserSettings = userModel.UserSettings;
            ApiKey = userModel.ApiKey;
            OldPassword = userModel.OldPassword;
            ChangedPassword = userModel.ChangedPassword;
            ChangedPasswordValidator = userModel.ChangedPasswordValidator;
            AfterResetPassword = userModel.AfterResetPassword;
            AfterResetPasswordValidator = userModel.AfterResetPasswordValidator;
            PasswordHistries = userModel.PasswordHistries;
            MailAddresses = userModel.MailAddresses;
            DemoMailAddress = userModel.DemoMailAddress;
            SessionGuid = userModel.SessionGuid;
            SecondaryAuthenticationCode = userModel.SecondaryAuthenticationCode;
            SecondaryAuthenticationCodeExpirationTime = userModel.SecondaryAuthenticationCodeExpirationTime;
            LdapSearchRoot = userModel.LdapSearchRoot;
            SynchronizedTime = userModel.SynchronizedTime;
            SecretKey = userModel.SecretKey;
            EnableSecretKey = userModel.EnableSecretKey;
            LoginExpirationLimit = userModel.LoginExpirationLimit;
            LoginExpirationPeriod = userModel.LoginExpirationPeriod;
            Comments = userModel.Comments;
            Creator = userModel.Creator;
            Updator = userModel.Updator;
            CreatedTime = userModel.CreatedTime;
            UpdatedTime = userModel.UpdatedTime;
            VerUp = userModel.VerUp;
            Comments = userModel.Comments;
            ClassHash = userModel.ClassHash;
            NumHash = userModel.NumHash;
            DateHash = userModel.DateHash;
            DescriptionHash = userModel.DescriptionHash;
            CheckHash = userModel.CheckHash;
            AttachmentsHash = userModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss, UserApiModel data)
        {
            if (data.LoginId != null) LoginId = data.LoginId.ToString().ToString();
            if (data.GlobalId != null) GlobalId = data.GlobalId.ToString().ToString();
            if (data.Name != null) Name = data.Name.ToString().ToString();
            if (data.UserCode != null) UserCode = data.UserCode.ToString().ToString();
            if (data.Password != null) Password = data.Password.ToString().ToString().Sha512Cng();
            if (data.LastName != null) LastName = data.LastName.ToString().ToString();
            if (data.FirstName != null) FirstName = data.FirstName.ToString().ToString();
            if (data.Birthday != null) Birthday = new Time(context, data.Birthday.ToDateTime(), byForm: true);
            if (data.Gender != null) Gender = data.Gender.ToString().ToString();
            if (data.Language != null) Language = data.Language.ToString().ToString();
            if (data.TimeZone != null) TimeZone = data.TimeZone.ToString().ToString();
            if (data.DeptCode != null) DeptCode = data.DeptCode.ToString().ToString();
            if (data.DeptId != null) DeptId = data.DeptId.ToInt().ToInt();
            if (data.Theme != null) Theme = data.Theme.ToString().ToString();
            if (data.FirstAndLastNameOrder != null) FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)data.FirstAndLastNameOrder.ToInt().ToInt();
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.LastLoginTime != null) LastLoginTime = new Time(context, data.LastLoginTime.ToDateTime(), byForm: true);
            if (data.PasswordExpirationTime != null) PasswordExpirationTime = new Time(context, data.PasswordExpirationTime.ToDateTime(), byForm: true);
            if (data.PasswordChangeTime != null) PasswordChangeTime = new Time(context, data.PasswordChangeTime.ToDateTime(), byForm: true);
            if (data.NumberOfLogins != null) NumberOfLogins = data.NumberOfLogins.ToInt().ToInt();
            if (data.NumberOfDenial != null) NumberOfDenial = data.NumberOfDenial.ToInt().ToInt();
            if (data.TenantManager != null) TenantManager = data.TenantManager.ToBool().ToBool();
            if (data.AllowCreationAtTopSite != null) AllowCreationAtTopSite = data.AllowCreationAtTopSite.ToBool().ToBool();
            if (data.AllowGroupAdministration != null) AllowGroupAdministration = data.AllowGroupAdministration.ToBool().ToBool();
            if (data.AllowGroupCreation != null) AllowGroupCreation = data.AllowGroupCreation.ToBool().ToBool();
            if (data.AllowApi != null) AllowApi = data.AllowApi.ToBool().ToBool();
            if (data.AllowMovingFromTopSite != null) AllowMovingFromTopSite = data.AllowMovingFromTopSite.ToBool().ToBool();
            if (data.EnableSecondaryAuthentication != null) EnableSecondaryAuthentication = data.EnableSecondaryAuthentication.ToBool().ToBool();
            if (data.DisableSecondaryAuthentication != null) DisableSecondaryAuthentication = data.DisableSecondaryAuthentication.ToBool().ToBool();
            if (data.Disabled != null) Disabled = data.Disabled.ToBool().ToBool();
            if (data.Lockout != null) Lockout = data.Lockout.ToBool().ToBool();
            if (data.LockoutCounter != null) LockoutCounter = data.LockoutCounter.ToInt().ToInt();
            if (data.SecondaryAuthenticationCode != null) SecondaryAuthenticationCode = data.SecondaryAuthenticationCode.ToString().ToString().Sha512Cng();
            if (data.SecondaryAuthenticationCodeExpirationTime != null) SecondaryAuthenticationCodeExpirationTime = new Time(context, data.SecondaryAuthenticationCodeExpirationTime.ToDateTime(), byForm: true);
            if (data.LdapSearchRoot != null) LdapSearchRoot = data.LdapSearchRoot.ToString().ToString();
            if (data.SynchronizedTime != null) SynchronizedTime = data.SynchronizedTime.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.SecretKey != null) SecretKey = data.SecretKey.ToString().ToString();
            if (data.EnableSecretKey != null) EnableSecretKey = data.EnableSecretKey.ToBool().ToBool();
            if (data.LoginExpirationLimit != null) LoginExpirationLimit = new Time(context, data.LoginExpirationLimit.ToDateTime(), byForm: true);
            if (data.LoginExpirationPeriod != null) LoginExpirationPeriod = data.LoginExpirationPeriod.ToInt().ToInt();
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            data.ClassHash?.ForEach(o => SetClass(
                columnName: o.Key,
                value: o.Value));
            data.NumHash?.ForEach(o => SetNum(
                columnName: o.Key,
                value: new Num(
                    context: context,
                    column: ss.GetColumn(
                        context: context,
                        columnName: o.Key),
                    value: o.Value.ToString())));
            data.DateHash?.ForEach(o => SetDate(
                columnName: o.Key,
                value: o.Value.ToDateTime().ToUniversal(context: context)));
            data.DescriptionHash?.ForEach(o => SetDescription(
                columnName: o.Key,
                value: o.Value));
            data.CheckHash?.ForEach(o => SetCheck(
                columnName: o.Key,
                value: o.Value));
            data.AttachmentsHash?.ForEach(o =>
            {
                string columnName = o.Key;
                Attachments newAttachments = o.Value;
                Attachments oldAttachments;
                if (columnName == "Attachments#Uploading")
                {
                    var kvp = AttachmentsHash
                        .FirstOrDefault(x => x.Value
                            .Any(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st()));
                    columnName = kvp.Key;
                    oldAttachments = kvp.Value;
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    if (column.OverwriteSameFileName == true)
                    {
                        var oldAtt = oldAttachments
                            .FirstOrDefault(att => att.Guid == newAttachments.FirstOrDefault()?.Guid?.Split_1st());
                        if (oldAtt != null)
                        {
                            oldAtt.Deleted = true;
                            oldAtt.Overwritten = true;
                        }
                    }
                    newAttachments.ForEach(att => att.Guid = att.Guid.Split_2nd());
                }
                else
                {
                    oldAttachments = AttachmentsHash.Get(columnName);
                }
                if (oldAttachments != null)
                {
                    var column = ss.GetColumn(
                        context: context,
                        columnName: columnName);
                    var newGuidSet = new HashSet<string>(newAttachments.Select(x => x.Guid).Distinct());
                    var newNameSet = new HashSet<string>(newAttachments.Select(x => x.Name).Distinct());
                    newAttachments.ForEach(newAttachment =>
                    {
                        newAttachment.AttachmentAction(
                            context: context,
                            column: column,
                            oldAttachments: oldAttachments);
                    });
                    if (column.OverwriteSameFileName == true)
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) =>
                                !newGuidSet.Contains(oldvalue.Guid) &&
                                !newNameSet.Contains(oldvalue.Name)));
                    }
                    else
                    {
                        newAttachments.AddRange(oldAttachments.
                            Where((oldvalue) => !newGuidSet.Contains(oldvalue.Guid)));
                    }
                }
                SetAttachments(columnName: columnName, value: newAttachments);
            });
        }

        public string ReplacedDisplayValues(
            Context context,
            SiteSettings ss,
            string value)
        {
            ss.IncludedColumns(value: value).ForEach(column =>
                value = value.Replace(
                    $"[{column.ColumnName}]",
                    ToDisplay(
                        context: context,
                        ss: ss,
                        column: column,
                        mine: Mine(context: context))));
            value = ReplacedContextValues(context, value);
            return value;
        }

        private string ReplacedContextValues(Context context, string value)
        {
            var url = Locations.ItemEditAbsoluteUri(
                context: context,
                id: UserId);
            var mailAddress = MailAddressUtilities.Get(
                context: context,
                userId: context.UserId);
            value = value
                .Replace("{Url}", url)
                .Replace("{LoginId}", context.User.LoginId)
                .Replace("{UserName}", context.User.Name)
                .Replace("{MailAddress}", mailAddress);
            return value;
        }

        private void SetBySession(Context context)
        {
            if (!context.Forms.Exists("Users_UserSettings")) UserSettings = Session_UserSettings(context: context);
            if (!context.Forms.Exists("Users_MailAddresses")) MailAddresses = Session_MailAddresses(context: context);
        }

        private void Set(Context context, SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(context, ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach (DataColumn dataColumn in dataRow.Table.Columns)
            {
                var column = new ColumnNameInfo(dataColumn.ColumnName);
                if (column.TableAlias == tableAlias)
                {
                    switch (column.Name)
                    {
                        case "TenantId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                TenantId = dataRow[column.ColumnName].ToInt();
                                SavedTenantId = TenantId;
                            }
                            break;
                        case "UserId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                UserId = dataRow[column.ColumnName].ToInt();
                                SavedUserId = UserId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "LoginId":
                            LoginId = dataRow[column.ColumnName].ToString();
                            SavedLoginId = LoginId;
                            break;
                        case "GlobalId":
                            GlobalId = dataRow[column.ColumnName].ToString();
                            SavedGlobalId = GlobalId;
                            break;
                        case "Name":
                            Name = dataRow[column.ColumnName].ToString();
                            SavedName = Name;
                            break;
                        case "UserCode":
                            UserCode = dataRow[column.ColumnName].ToString();
                            SavedUserCode = UserCode;
                            break;
                        case "Password":
                            Password = dataRow[column.ColumnName].ToString();
                            SavedPassword = Password;
                            break;
                        case "LastName":
                            LastName = dataRow[column.ColumnName].ToString();
                            SavedLastName = LastName;
                            break;
                        case "FirstName":
                            FirstName = dataRow[column.ColumnName].ToString();
                            SavedFirstName = FirstName;
                            break;
                        case "Birthday":
                            Birthday = new Time(context, dataRow, column.ColumnName);
                            SavedBirthday = Birthday.Value;
                            break;
                        case "Gender":
                            Gender = dataRow[column.ColumnName].ToString();
                            SavedGender = Gender;
                            break;
                        case "Language":
                            Language = dataRow[column.ColumnName].ToString();
                            SavedLanguage = Language;
                            break;
                        case "TimeZone":
                            TimeZone = dataRow[column.ColumnName].ToString();
                            SavedTimeZone = TimeZone;
                            break;
                        case "DeptCode":
                            DeptCode = dataRow[column.ColumnName].ToString();
                            SavedDeptCode = DeptCode;
                            break;
                        case "DeptId":
                            DeptId = dataRow[column.ColumnName].ToInt();
                            SavedDeptId = DeptId;
                            break;
                        case "Theme":
                            Theme = dataRow[column.ColumnName].ToString();
                            SavedTheme = Theme;
                            break;
                        case "FirstAndLastNameOrder":
                            FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)dataRow[column.ColumnName].ToInt();
                            SavedFirstAndLastNameOrder = FirstAndLastNameOrder.ToInt();
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
                            break;
                        case "LastLoginTime":
                            LastLoginTime = new Time(context, dataRow, column.ColumnName);
                            SavedLastLoginTime = LastLoginTime.Value;
                            break;
                        case "PasswordExpirationTime":
                            PasswordExpirationTime = new Time(context, dataRow, column.ColumnName);
                            SavedPasswordExpirationTime = PasswordExpirationTime.Value;
                            break;
                        case "PasswordChangeTime":
                            PasswordChangeTime = new Time(context, dataRow, column.ColumnName);
                            SavedPasswordChangeTime = PasswordChangeTime.Value;
                            break;
                        case "NumberOfLogins":
                            NumberOfLogins = dataRow[column.ColumnName].ToInt();
                            SavedNumberOfLogins = NumberOfLogins;
                            break;
                        case "NumberOfDenial":
                            NumberOfDenial = dataRow[column.ColumnName].ToInt();
                            SavedNumberOfDenial = NumberOfDenial;
                            break;
                        case "TenantManager":
                            TenantManager = dataRow[column.ColumnName].ToBool();
                            SavedTenantManager = TenantManager;
                            break;
                        case "ServiceManager":
                            ServiceManager = dataRow[column.ColumnName].ToBool();
                            SavedServiceManager = ServiceManager;
                            break;
                        case "AllowCreationAtTopSite":
                            AllowCreationAtTopSite = dataRow[column.ColumnName].ToBool();
                            SavedAllowCreationAtTopSite = AllowCreationAtTopSite;
                            break;
                        case "AllowGroupAdministration":
                            AllowGroupAdministration = dataRow[column.ColumnName].ToBool();
                            SavedAllowGroupAdministration = AllowGroupAdministration;
                            break;
                        case "AllowGroupCreation":
                            AllowGroupCreation = dataRow[column.ColumnName].ToBool();
                            SavedAllowGroupCreation = AllowGroupCreation;
                            break;
                        case "AllowApi":
                            AllowApi = dataRow[column.ColumnName].ToBool();
                            SavedAllowApi = AllowApi;
                            break;
                        case "AllowMovingFromTopSite":
                            AllowMovingFromTopSite = dataRow[column.ColumnName].ToBool();
                            SavedAllowMovingFromTopSite = AllowMovingFromTopSite;
                            break;
                        case "EnableSecondaryAuthentication":
                            EnableSecondaryAuthentication = dataRow[column.ColumnName].ToBool();
                            SavedEnableSecondaryAuthentication = EnableSecondaryAuthentication;
                            break;
                        case "DisableSecondaryAuthentication":
                            DisableSecondaryAuthentication = dataRow[column.ColumnName].ToBool();
                            SavedDisableSecondaryAuthentication = DisableSecondaryAuthentication;
                            break;
                        case "Disabled":
                            Disabled = dataRow[column.ColumnName].ToBool();
                            SavedDisabled = Disabled;
                            break;
                        case "Lockout":
                            Lockout = dataRow[column.ColumnName].ToBool();
                            SavedLockout = Lockout;
                            break;
                        case "LockoutCounter":
                            LockoutCounter = dataRow[column.ColumnName].ToInt();
                            SavedLockoutCounter = LockoutCounter;
                            break;
                        case "Developer":
                            Developer = dataRow[column.ColumnName].ToBool();
                            SavedDeveloper = Developer;
                            break;
                        case "UserSettings":
                            UserSettings = GetUserSettings(dataRow);
                            SavedUserSettings = UserSettings.RecordingJson();
                            break;
                        case "ApiKey":
                            ApiKey = dataRow[column.ColumnName].ToString();
                            SavedApiKey = ApiKey;
                            break;
                        case "PasswordHistries":
                            PasswordHistries = dataRow[column.ColumnName].ToString().Deserialize<List<PasswordHistory>>() ?? new List<PasswordHistory>();
                            SavedPasswordHistries = PasswordHistries.ToJson();
                            break;
                        case "SecondaryAuthenticationCode":
                            SecondaryAuthenticationCode = dataRow[column.ColumnName].ToString();
                            SavedSecondaryAuthenticationCode = SecondaryAuthenticationCode;
                            break;
                        case "SecondaryAuthenticationCodeExpirationTime":
                            SecondaryAuthenticationCodeExpirationTime = new Time(context, dataRow, column.ColumnName);
                            SavedSecondaryAuthenticationCodeExpirationTime = SecondaryAuthenticationCodeExpirationTime.Value;
                            break;
                        case "LdapSearchRoot":
                            LdapSearchRoot = dataRow[column.ColumnName].ToString();
                            SavedLdapSearchRoot = LdapSearchRoot;
                            break;
                        case "SynchronizedTime":
                            SynchronizedTime = dataRow[column.ColumnName].ToDateTime();
                            SavedSynchronizedTime = SynchronizedTime;
                            break;
                        case "SecretKey":
                            SecretKey = dataRow[column.ColumnName].ToString();
                            SavedSecretKey = SecretKey;
                            break;
                        case "EnableSecretKey":
                            EnableSecretKey = dataRow[column.ColumnName].ToBool();
                            SavedEnableSecretKey = EnableSecretKey;
                            break;
                        case "LoginExpirationLimit":
                            LoginExpirationLimit = new Time(context, dataRow, column.ColumnName);
                            SavedLoginExpirationLimit = LoginExpirationLimit.Value;
                            break;
                        case "LoginExpirationPeriod":
                            LoginExpirationPeriod = dataRow[column.ColumnName].ToInt();
                            SavedLoginExpirationPeriod = LoginExpirationPeriod;
                            break;
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(context: context, userId: dataRow.Int(column.ColumnName));
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(context, dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(context, dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory":
                            VerType = dataRow.Bool(column.ColumnName)
                                ? Versions.VerTypes.History
                                : Versions.VerTypes.Latest; break;
                        default:
                            switch (Def.ExtendedColumnTypes.Get(column?.Name ?? string.Empty))
                            {
                                case "Class":
                                    SetClass(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedClass(
                                        columnName: column.Name,
                                        value: GetClass(columnName: column.Name));
                                    break;
                                case "Num":
                                    SetNum(
                                        columnName: column.Name,
                                        value: new Num(
                                            dataRow: dataRow,
                                            name: column.ColumnName));
                                    SetSavedNum(
                                        columnName: column.Name,
                                        value: GetNum(columnName: column.Name).Value);
                                    break;
                                case "Date":
                                    SetDate(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SetSavedDate(
                                        columnName: column.Name,
                                        value: GetDate(columnName: column.Name));
                                    break;
                                case "Description":
                                    SetDescription(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SetSavedDescription(
                                        columnName: column.Name,
                                        value: GetDescription(columnName: column.Name));
                                    break;
                                case "Check":
                                    SetCheck(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SetSavedCheck(
                                        columnName: column.Name,
                                        value: GetCheck(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    SetAttachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SetSavedAttachments(
                                        columnName: column.Name,
                                        value: GetAttachments(columnName: column.Name).ToJson());
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return Updated()
                || TenantId_Updated(context: context)
                || UserId_Updated(context: context)
                || Ver_Updated(context: context)
                || LoginId_Updated(context: context)
                || GlobalId_Updated(context: context)
                || Name_Updated(context: context)
                || UserCode_Updated(context: context)
                || Password_Updated(context: context)
                || LastName_Updated(context: context)
                || FirstName_Updated(context: context)
                || Birthday_Updated(context: context)
                || Gender_Updated(context: context)
                || Language_Updated(context: context)
                || TimeZone_Updated(context: context)
                || DeptId_Updated(context: context)
                || Theme_Updated(context: context)
                || FirstAndLastNameOrder_Updated(context: context)
                || Body_Updated(context: context)
                || LastLoginTime_Updated(context: context)
                || PasswordExpirationTime_Updated(context: context)
                || PasswordChangeTime_Updated(context: context)
                || NumberOfLogins_Updated(context: context)
                || NumberOfDenial_Updated(context: context)
                || TenantManager_Updated(context: context)
                || ServiceManager_Updated(context: context)
                || AllowCreationAtTopSite_Updated(context: context)
                || AllowGroupAdministration_Updated(context: context)
                || AllowGroupCreation_Updated(context: context)
                || AllowApi_Updated(context: context)
                || AllowMovingFromTopSite_Updated(context: context)
                || EnableSecondaryAuthentication_Updated(context: context)
                || DisableSecondaryAuthentication_Updated(context: context)
                || Disabled_Updated(context: context)
                || Lockout_Updated(context: context)
                || LockoutCounter_Updated(context: context)
                || Developer_Updated(context: context)
                || UserSettings_Updated(context: context)
                || ApiKey_Updated(context: context)
                || PasswordHistries_Updated(context: context)
                || SecondaryAuthenticationCode_Updated(context: context)
                || SecondaryAuthenticationCodeExpirationTime_Updated(context: context)
                || LdapSearchRoot_Updated(context: context)
                || SynchronizedTime_Updated(context: context)
                || SecretKey_Updated(context: context)
                || EnableSecretKey_Updated(context: context)
                || LoginExpirationLimit_Updated(context: context)
                || LoginExpirationPeriod_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        private bool UpdatedWithColumn(Context context, SiteSettings ss)
        {
            return ClassHash.Any(o => Class_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || NumHash.Any(o => Num_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || DateHash.Any(o => Date_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || DescriptionHash.Any(o => Description_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || CheckHash.Any(o => Check_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)))
                || AttachmentsHash.Any(o => Attachments_Updated(
                    columnName: o.Key,
                    column: ss.GetColumn(context: context, o.Key)));
        }

        public bool Updated(Context context, SiteSettings ss)
        {
            return UpdatedWithColumn(context: context, ss: ss)
                || TenantId_Updated(context: context)
                || UserId_Updated(context: context)
                || Ver_Updated(context: context)
                || LoginId_Updated(context: context)
                || GlobalId_Updated(context: context)
                || Name_Updated(context: context)
                || UserCode_Updated(context: context)
                || Password_Updated(context: context)
                || LastName_Updated(context: context)
                || FirstName_Updated(context: context)
                || Birthday_Updated(context: context)
                || Gender_Updated(context: context)
                || Language_Updated(context: context)
                || TimeZone_Updated(context: context)
                || DeptId_Updated(context: context)
                || Theme_Updated(context: context)
                || FirstAndLastNameOrder_Updated(context: context)
                || Body_Updated(context: context)
                || LastLoginTime_Updated(context: context)
                || PasswordExpirationTime_Updated(context: context)
                || PasswordChangeTime_Updated(context: context)
                || NumberOfLogins_Updated(context: context)
                || NumberOfDenial_Updated(context: context)
                || TenantManager_Updated(context: context)
                || ServiceManager_Updated(context: context)
                || AllowCreationAtTopSite_Updated(context: context)
                || AllowGroupAdministration_Updated(context: context)
                || AllowGroupCreation_Updated(context: context)
                || AllowApi_Updated(context: context)
                || AllowMovingFromTopSite_Updated(context: context)
                || EnableSecondaryAuthentication_Updated(context: context)
                || DisableSecondaryAuthentication_Updated(context: context)
                || Disabled_Updated(context: context)
                || Lockout_Updated(context: context)
                || LockoutCounter_Updated(context: context)
                || Developer_Updated(context: context)
                || UserSettings_Updated(context: context)
                || ApiKey_Updated(context: context)
                || PasswordHistries_Updated(context: context)
                || SecondaryAuthenticationCode_Updated(context: context)
                || SecondaryAuthenticationCodeExpirationTime_Updated(context: context)
                || LdapSearchRoot_Updated(context: context)
                || SynchronizedTime_Updated(context: context)
                || SecretKey_Updated(context: context)
                || EnableSecretKey_Updated(context: context)
                || LoginExpirationLimit_Updated(context: context)
                || LoginExpirationPeriod_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        public override List<string> Mine(Context context)
        {
            if (MineCache == null)
            {
                var mine = new List<string>();
                var userId = context.UserId;
                if (SavedCreator == userId) mine.Add("Creator");
                if (SavedUpdator == userId) mine.Add("Updator");
                MineCache = mine;
            }
            return MineCache;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetTimeZoneAndLanguage(
            Context context,
            SiteSettings ss,
            MethodTypes methodType)
        {
            if (methodType == MethodTypes.New)
            {
                var tenantModel = new TenantModel(
                    context: context,
                    ss: ss,
                    tenantId: TenantId);
                Language = tenantModel.Language.IsNullOrEmpty()
                    ? Parameters.Service.DefaultLanguage
                    : tenantModel.Language;
                TimeZone = tenantModel.TimeZone.IsNullOrEmpty()
                    ? Parameters.Service.TimeZoneDefault
                    : tenantModel.TimeZone;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private UserSettings GetUserSettings(DataRow dataRow)
        {
            return dataRow["UserSettings"].ToString().Deserialize<UserSettings>() ??
                new UserSettings();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateMailAddresses(Context context)
        {
            if (UserId > 0)
            {
                var statements = new List<SqlStatement>();
                Session_MailAddresses(context: context)?.ForEach(mailAddress =>
                    statements.Add(Rds.InsertMailAddresses(
                        param: Rds.MailAddressesParam()
                            .OwnerId(UserId)
                            .OwnerType("Users")
                            .MailAddress(mailAddress))));
                statements.Insert(0, Rds.PhysicalDeleteMailAddresses(
                    where: Rds.MailAddressesWhere()
                        .OwnerId(UserId)
                        .OwnerType("Users")));
                Repository.ExecuteNonQuery(
                    context: context,
                    transactional: true,
                    statements: statements.ToArray());
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateMailAddresses(Context context, UserApiModel userApiModel)
        {
            if (UserId > 0)
            {
                var mailAddresses = userApiModel.MailAddresses
                    .OrderBy(o => o)
                    .Join();
                var where = Rds.MailAddressesWhere()
                    .OwnerId(UserId)
                    .OwnerType("Users");
                if (new MailAddressCollection(
                    context: context,
                    where: where)
                        .Select(o => o.MailAddress)
                        .OrderBy(o => o)
                        .Join() != mailAddresses)
                {
                    var statements = new List<SqlStatement>()
                    {
                        Rds.PhysicalDeleteMailAddresses(where: where)
                    };
                    userApiModel.MailAddresses
                        .Where(mailAddress => !Libraries.Mails.Addresses.GetBody(mailAddress).IsNullOrEmpty())
                        .ForEach(mailAddress =>
                            statements.Add(Rds.InsertMailAddresses(param: Rds.MailAddressesParam()
                                .OwnerId(UserId)
                                .OwnerType("Users")
                                .MailAddress(mailAddress))));
                    Repository.ExecuteNonQuery(
                        context: context,
                        transactional: true,
                        statements: statements.ToArray());
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private List<string> GetMailAddresses(Context context, SiteSettings ss, int? userId)
        {
            var mailAddresses = Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectMailAddresses(
                    column: Rds.MailAddressesColumn()
                        .MailAddressId()
                        .MailAddress(),
                    where: Rds.MailAddressesWhere()
                        .OwnerId(userId)
                        .OwnerType("Users")))
                            .AsEnumerable()
                            .Select(o => o["MailAddress"].ToString())
                            .ToList();
            return mailAddresses;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetPasswordExpirationPeriod(Context context)
        {
            PasswordExpirationTime = Parameters.Security.PasswordExpirationPeriod != 0
                ? new Time(
                    context: context,
                    value: DateTime.Today.AddDays(Parameters.Security.PasswordExpirationPeriod))
                : PasswordExpirationTime;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public UserModel(
            Context context,
            SiteSettings ss,
            string loginId,
            Dictionary<string, string> formData = null)
        {
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (!loginId.IsNullOrEmpty())
            {
                Get(
                    context: context,
                    ss: ss,
                    where: Rds.UsersWhere()
                        .TenantId(context.TenantId)
                        .LoginId(loginId));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool Self(Context context)
        {
            return UserId == context.UserId;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Authenticate(Context context, string returnUrl, bool isAuthenticationByMail, bool noHttpContext = false)
        {
            if (Parameters.Security?.SecondaryAuthentication?.NotificationType == ParameterAccessor.Parts.SecondaryAuthentication.SecondaryAuthenticationModeNotificationTypes.Mail)
            {
                isAuthenticationByMail = true;
            }
            if (Parameters.Security.RevealUserDisabled && DisabledUser(context: context))
            {
                return UserDisabled(context: context);
            }
            if (RejectUnregisteredUser(context: context))
            {
                return Deny(context: context);
            }
            if (Authenticate(context: context))
            {
                if (LoginExpired())
                {
                    return DenyLoginExpired(context: context);
                }
                if (!AllowedIpAddress(context))
                {
                    return InvalidIpAddress(context: context);
                }
                else if (Lockout)
                {
                    return UserLockout(context: context);
                }
                else if (EnabledSecondaryAuthentication(context: context))
                {
                    var secondaryAuthenticationCode = context
                        .Forms
                        .Data("SecondaryAuthenticationCode");
                    return string.IsNullOrEmpty(secondaryAuthenticationCode)
                        ? !EnableSecretKey && !isAuthenticationByMail
                            ? OpenTotpRegisterCode(context: context)
                            : OpenSecondaryAuthentication(
                                context: context,
                                returnUrl: returnUrl,
                                isAuthenticationByMail: isAuthenticationByMail)
                        : !SecondaryAuthentication(
                                context: context,
                                secondaryAuthenticationCode: secondaryAuthenticationCode,
                                isAuthenticationByMail: isAuthenticationByMail)
                            ? Messages
                                .ResponseSecondaryAuthentication(
                                    context: context,
                                    target: "#LoginMessage")
                                .Focus("#SecondaryAuthenticationCode")
                                .ToJson()
                            : PasswordExpired()
                                ? OpenChangePasswordAtLoginDialog(context: context)
                                : !EnableSecretKey && !isAuthenticationByMail
                                    ? setEnableSecretKeyandAllow(
                                        context: context,
                                        returnUrl: returnUrl,
                                        createPersistentCookie: context.Forms.Bool("Users_RememberMe"),
                                        noHttpContext: noHttpContext)
                                    : Allow(
                                        context: context,
                                        returnUrl: returnUrl,
                                        createPersistentCookie: context.Forms.Bool("Users_RememberMe"),
                                        noHttpContext: noHttpContext);
                }
                else if (PasswordExpired())
                {
                    return OpenChangePasswordAtLoginDialog(context: context);
                }
                else
                {
                    return Allow(
                        context: context,
                        returnUrl: returnUrl,
                        createPersistentCookie: context.Forms.Bool("Users_RememberMe"),
                        noHttpContext: noHttpContext);
                }
            }
            else
            {
                var tenantOptions = TenantOptions(context: context);
                if (tenantOptions?.Any() == true)
                {
                    return TenantsDropDown(
                        context: context,
                        tenantOptions: tenantOptions);
                }
                else
                {
                    return Deny(context: context);
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool Authenticate(Context context)
        {
            bool authenticated;
            switch (Parameters.Authentication.Provider)
            {
                case "LDAP":
                case "Windows":
                    authenticated = Ldap.Authenticate(
                        context: context,
                        loginId: LoginId,
                        password: context.Forms.Data("Users_Password"));
                    if (authenticated)
                    {
                        Get(
                            context: context,
                            ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                            where: Rds.UsersWhere().LoginId(
                                value: context.Sqls.EscapeValue(LoginId),
                                _operator: context.Sqls.LikeWithEscape));
                    }
                    break;
                case "LDAP+Local":
                    authenticated = Ldap.Authenticate(
                        context: context,
                        loginId: LoginId,
                        password: context.Forms.Data("Users_Password"));
                    if (authenticated)
                    {
                        Get(
                            context: context,
                            ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                            where: Rds.UsersWhere().LoginId(
                                value: context.Sqls.EscapeValue(LoginId),
                                _operator: context.Sqls.LikeWithEscape));
                    }
                    else
                    {
                        authenticated = GetByCredentials(
                            context: context,
                            loginId: LoginId,
                            password: Password,
                            tenantId: context.Forms.Int("SelectedTenantId"));
                    }
                    break;
                case "Extension":
                    authenticated = Extension.Authenticate(
                        context: context,
                        loginId: LoginId,
                        password: Password,
                        userModel: this);
                    break;
                case "SAML":
                    authenticated = GetByCredentials(
                        context: context,
                        loginId: LoginId,
                        password: Password,
                        tenantId: context.Forms.Int("SelectedTenantId"));
                    if (authenticated)
                    {
                        context = new Context(tenantId: TenantId);
                        if (context.ContractSettings?.AllowOriginalLogin == 0 && TenantManager == false)
                        {
                            authenticated = false;
                        }
                    }
                    break;
                default:
                    authenticated = GetByCredentials(
                        context: context,
                        loginId: LoginId,
                        password: Password,
                        tenantId: context.Forms.Int("SelectedTenantId"));
                    break;
            }
            UpdateLockout(
                context: context,
                loginId: LoginId,
                authenticated: authenticated);
            return authenticated;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool AllowedIpAddress(Context context)
        {
            var createdContext = new Context(
                tenantId: TenantId,
                context: context);
            return context.ContractSettings.AllowedIpAddress(
                context: context,
                createdContext.UserHostAddress);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool DisabledUser(Context context)
        {
            return Repository.ExecuteScalar_int(
                context: context,
                statements: Rds.SelectUsers(
                    column: Rds.UsersColumn().UsersCount(),
                    where: Rds.UsersWhere()
                    .Add(
                        name: "LoginId",
                        value: LoginId,
                        raw: "(lower(\"Users\".\"LoginId\") = lower(@LoginId))")
                        .Disabled(true))) == 1;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool RejectUnregisteredUser(Context context)
        {
            return Parameters.Authentication.RejectUnregisteredUser &&
                Repository.ExecuteScalar_int(
                    context: context,
                    statements: Rds.SelectUsers(
                        column: Rds.UsersColumn().UsersCount(),
                        where: Rds.UsersWhere()
                        .Add(
                            name: "LoginId",
                            value: LoginId,
                            raw: "(lower(\"Users\".\"LoginId\") = lower(@LoginId))")
                            .Disabled(false))) != 1;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool GetByCredentials(
            Context context,
            string loginId,
            string password,
            int tenantId = 0,
            bool updateLockout = false)
        {
            Get(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                where: Rds.UsersWhere()
                    .TenantId(tenantId, _using: tenantId > 0)
                    .LoginId(
                        value: context.Sqls.EscapeValue(loginId),
                        _operator: context.Sqls.LikeWithEscape)
                    .Password(password)
                    .Disabled(false));
            var authenticated = AccessStatus == Databases.AccessStatuses.Selected;
            if (updateLockout)
            {
                UpdateLockout(
                    context: context,
                    loginId: loginId,
                    authenticated: authenticated);
            }
            return authenticated;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void UpdateLockout(Context context, string loginId, bool authenticated)
        {
            if (Parameters.Security.LockoutCount > 0)
            {
                if (authenticated)
                {
                    if (!Lockout)
                    {
                        // ロックアウトカウンターを0に戻す
                        Repository.ExecuteNonQuery(
                            context: context,
                            statements: Rds.UpdateUsers(
                                where: Rds.UsersWhere().LoginId(
                                    value: context.Sqls.EscapeValue(loginId),
                                    _operator: context.Sqls.LikeWithEscape),
                                param: Rds.UsersParam().LockoutCounter(0),
                                addUpdatorParam: false,
                                addUpdatedTimeParam: false));
                    }
                }
                else
                {
                    // ロックアウトカウンターをインクリメントする
                    // ロックアウトカウンターが Parameters.Security.LockoutCount に達した場合にはアカウントをロックする
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.UpdateUsers(
                            where: Rds.UsersWhere().LoginId(
                                value: context.Sqls.EscapeValue(loginId),
                                _operator: context.Sqls.LikeWithEscape),
                            param: Rds.UsersParam()
                                .LockoutCounter(raw: "\"Users\".\"LockoutCounter\"+1")
                                .Lockout(raw: "case when \"Users\".\"Lockout\"={1} or \"Users\".\"LockoutCounter\"+1>={0} then {1} else {2} end"
                                    .Params(
                                        Parameters.Security.LockoutCount,
                                        context.Sqls.TrueString,
                                        context.Sqls.FalseString)),
                            addUpdatorParam: false,
                            addUpdatedTimeParam: false));
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string TenantsDropDown(
            Context context, Dictionary<string, string> tenantOptions)
        {
            return new ResponseCollection(context: context)
                .Html("#Tenants", new HtmlBuilder().FieldDropDown(
                    context: context,
                    controlId: "SelectedTenantId",
                    fieldCss: " field-wide",
                    controlCss: " always-send",
                    labelText: Displays.Tenants(context: context),
                    optionCollection: tenantOptions)).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private Dictionary<string, string> TenantOptions(Context context)
        {
            return Repository.ExecuteScalar_string(
                context: context,
                statements: Rds.SelectLoginKeys(
                    column: Rds.LoginKeysColumn().TenantNames(),
                    where: Rds.LoginKeysWhere()
                        .LoginId(LoginId)))
                            .Deserialize<Dictionary<string, string>>();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private char[] AuthenticationCodeUsableCharactors(
            Authentications.AuthenticationCodeCharacterTypes authenticationCodeCharacterTypes)
        {
            switch (authenticationCodeCharacterTypes)
            {
                case Authentications.AuthenticationCodeCharacterTypes.Number:
                    return Enumerable.Range('0', '9' - '0' + 1).Select(o => (char)o).ToArray();
                case Authentications.AuthenticationCodeCharacterTypes.Letter:
                    return Enumerable
                        .Range('A', 'Z' - 'A' + 1)
                        .Concat(Enumerable.Range('a', 'z' - 'a' + 1))
                        .Select(o => (char)o)
                        .ToArray();
                default:
                    return Enumerable
                        .Range('0', '9' - '0' + 1)
                        .Concat(Enumerable.Range('A', 'Z' - 'A' + 1))
                        .Concat(Enumerable.Range('a', 'z' - 'a' + 1))
                        .Select(o => (char)o)
                        .ToArray();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string CreateSecondaryAuthenticationCode()
        {
            switch (Parameters.Security?.SecondaryAuthentication?.AuthenticationCodeCharacterType)
            {
                case "Number":
                    return CreateAuthenticationCode(
                        length: Parameters
                            .Security
                            ?.SecondaryAuthentication
                            ?.AuthenticationCodeLength ?? 0,
                        chars: AuthenticationCodeUsableCharactors(
                            authenticationCodeCharacterTypes: Authentications
                                .AuthenticationCodeCharacterTypes
                                .Number));
                case "Letter":
                    return CreateAuthenticationCode(
                        length: Parameters
                            .Security
                            ?.SecondaryAuthentication
                            ?.AuthenticationCodeLength ?? 0,
                        chars: AuthenticationCodeUsableCharactors(
                            authenticationCodeCharacterTypes: Authentications
                                .AuthenticationCodeCharacterTypes
                                .Letter));
                default:
                    return CreateAuthenticationCode(
                        length: Parameters
                            .Security
                            ?.SecondaryAuthentication
                            ?.AuthenticationCodeLength ?? 0,
                        chars: AuthenticationCodeUsableCharactors(
                            authenticationCodeCharacterTypes: Authentications
                                .AuthenticationCodeCharacterTypes
                                .NumberAndLetter));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string CreateAuthenticationCode(int length, char[] chars)
        {
            var authenticationCodeLength = Math.Max(Math.Min(length, 128), 1);
            var generateLength = Math.Max(Math.Min(length * 3, 128), 1);
            var authenticationCode = new System.Text.StringBuilder();
            while (authenticationCode.Length < authenticationCodeLength)
            {
                authenticationCode.Append(Strings
                    .NewGuid()
                    .Where(o => chars?.Contains(o) != false).ToArray());
            }
            return authenticationCode.ToString().Substring(0, authenticationCodeLength);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateSecondaryAuthenticationCode(Context context)
        {
            SecondaryAuthenticationCode = CreateSecondaryAuthenticationCode();
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(
                        context: context,
                        userModel: this),
                    param: UpdateSecondaryAuthenticationCodeParam(
                        context: context,
                        authenticationCode: SecondaryAuthenticationCode),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SqlParamCollection UpdateSecondaryAuthenticationCodeParam(
            Context context,
            string authenticationCode)
        {
            return Rds.UsersParam()
                .SecondaryAuthenticationCode(authenticationCode)
                .SecondaryAuthenticationCodeExpirationTime(DateTime.Now.AddSeconds(Parameters.
                    Security
                    ?.SecondaryAuthentication
                    ?.AuthenticationCodeExpirationPeriod ?? 0));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateSecretKey(Context context)
        {
            SecretKey = OtpNet.Base32Encoding.ToString(OtpNet.KeyGeneration.GenerateRandomKey(20));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(
                        context: context,
                        userModel: this),
                    param: Rds.UsersParam().SecretKey(SecretKey),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(
                        context: context,
                        userModel: this),
                    param: Rds.UsersParam().EnableSecretKey(EnableSecretKey),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateEnableSecretKey(Context context)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(
                        context: context,
                        userModel: this),
                    param: Rds.UsersParam().EnableSecretKey(EnableSecretKey),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string AddHyphenSecretKey()
        {
            string addHyphenSecretKey = string.Empty;
            foreach (System.Text.RegularExpressions.Match keys in System.Text.RegularExpressions.Regex.Matches(SecretKey, "(....)"))
            {
                addHyphenSecretKey += keys + "-";
            }
            return addHyphenSecretKey[..^1];
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string setEnableSecretKeyandAllow(Context context, string returnUrl, bool createPersistentCookie, bool noHttpContext)
        {
            if (!EnableSecretKey)
            {
                EnableSecretKey = true;
                UpdateEnableSecretKey(context);
            }
            return Allow(
                context: context,
                returnUrl: returnUrl,
                createPersistentCookie: context.Forms.Bool("Users_RememberMe"),
                noHttpContext: noHttpContext);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void NotificationSecondaryAuthenticationCode(Context context, bool isAuthenticationByMail)
        {
            var language = Language.IsNullOrEmpty()
                ? context.Language
                : Language;
            if(isAuthenticationByMail)
            {
                Repository.ExecuteTable(
                    context: context,
                    statements: Rds.SelectMailAddresses(
                        column: Rds.MailAddressesColumn().MailAddress(),
                        where: Rds.MailAddressesWhere().OwnerId(UserId).OwnerType("Users")))
                            ?.AsEnumerable()
                            .Select(o => o["MailAddress"].ToString())
                            .Where(o => !o.IsNullOrEmpty())
                            .ForEach(mailAddress => new OutgoingMailModel()
                            {
                                Title = new Title(Displays.Get(
                                    id: "SecondaryAuthenticationMailSubject",
                                    language: language)),
                                Body = Displays.Get(
                                    id: "SecondaryAuthenticationMailBody",
                                    language: language)
                                        .Replace(
                                            "[AuthenticationCode]",
                                            SecondaryAuthenticationCode),
                                From = MimeKit.MailboxAddress.Parse(Parameters
                                    .Mail
                                    .SupportFrom),
                                To = $"\"{Name}\" <{mailAddress}>",
                                Bcc = Parameters.Security.SecondaryAuthentication.NotificationMailBcc
                                    ? Parameters.Mail.SupportFrom
                                    : string.Empty
                            }.Send(
                                context: context,
                                ss: new SiteSettings()));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string OpenSecondaryAuthentication(Context context, string returnUrl, bool isAuthenticationByMail)
        {
            if (isAuthenticationByMail)
            {
                UpdateSecondaryAuthenticationCode(context: context);
                NotificationSecondaryAuthenticationCode(context: context, isAuthenticationByMail: isAuthenticationByMail);
            }
            var hb = new HtmlBuilder();
            hb
                .Div(
                    id: "SecondaryAuthenticationGuideTop",
                    action: () => hb.Raw(HtmlHtmls.ExtendedHtmls(
                        context: context,
                        id: "SecondaryAuthenticationGuideTop")))
                .Div(action: isAuthenticationByMail
                    ? () => hb
                        .FieldTextBox(
                            textType: isAuthenticationByMail
                                ? HtmlTypes.TextTypes.Password
                                : HtmlTypes.TextTypes.Normal,
                            controlId: "SecondaryAuthenticationCode",
                            controlCss: " focus always-send",
                            labelText: Displays.AuthenticationCode(context: context),
                            validateRequired: true)
                    : () => hb = TotpAuthenticationCodeSeparate(hb, context));
            hb
                .Div(
                    id: "AuthenticationByMail",
                    attributes: new HtmlAttributes().Add(
                    "data-isAuthenticationByMail",
                    isAuthenticationByMail ? "1" : "0"),
                    action: isAuthenticationByMail
                        ? null
                        : () => hb.A(
                            href: "javascript:void(0)",
                            text: Displays.RequireSecondAuthenticationByMail(context: context)))
                                .Div(
                                    id: "SecondaryAuthenticationCommands",
                                    css: "both",
                                    action: () => hb.Div(css: "command-right", action: () => hb
                                        .Button(
                                            controlId: "SecondaryAuthenticate",
                                            controlCss: " button-icon validate",
                                            text: Displays.Confirm(context: context),
                                            onClick: "$p.send($(this));",
                                            icon: "ui-icon-unlocked",
                                            action: "Authenticate",
                                            method: "post",
                                            type: "submit")
                                        .Button(
                                                text: Displays.Cancel(context: context),
                                                controlCss: "button-icon ",
                                                onClick: "$p.back();",
                                                icon: "ui-icon-cancel")))
                                .Div(
                                    id: "SecondaryAuthenticationBottom",
                                    action: () => hb.Raw(HtmlHtmls.ExtendedHtmls(
                                        context: context,
                                        id: "SecondaryAuthenticationGuideBottom")));
            var rc = new ResponseCollection(context: context)
                .Css(
                    target: "#Logins",
                    name: "display",
                    value: "none")
                .Html(
                    target: "#SecondaryAuthentications",
                    value: hb)
                .Val(
                    target: "#BackUrl",
                    value: context.UrlReferrer,
                    _using: !context.UrlReferrer.IsNullOrEmpty());
            return isAuthenticationByMail
                ? rc.Focus("#SecondaryAuthenticationCode").ToJson()
                : rc.Focus("#FirstTotpAuthenticationCode").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string OpenTotpRegisterCode(Context context)
        {
            UpdateSecretKey(context);
            var hb = new HtmlBuilder();
            return new ResponseCollection(context: context)
                .Css(
                    target: "#Logins",
                    name: "display",
                    value: "none")
                .Html(
                    target: "#TotpRegister",
                    value: hb
                        .Div(
                            id: "TotpRegisterGuideTop",
                            action: () => hb.Raw(HtmlHtmls.ExtendedHtmls(
                                context: context,
                                id: "TotpRegisterGuideTop")))
                        .Div(
                            id: "TotpQRCode",
                            attributes: new HtmlAttributes().Add("data-url", "otpauth://totp/" + LoginId + "?secret=" + SecretKey + "&issuer=Implem%20Plesanter"),
                            action: () => hb.Span(
                                id: "qrCode"))
                        .Div(
                            id: "TotpQRCodeText",
                            action: () => hb.Span(
                                id: "qrCodeText",
                                action: () => hb.Text(AddHyphenSecretKey())))
                        .Div(action: () => hb = TotpAuthenticationCodeSeparate(hb, context))
                        .Div(
                            id: "AuthenticationByMail",
                            attributes: new HtmlAttributes().Add(
                                "data-isAuthenticationByMail", "0"),
                            action: () => hb.A(
                                href: "javascript:void(0)",
                                text: Displays.RequireSecondAuthenticationByMail(context: context)))
                                    .Div(
                                        id: "SecondaryAuthenticationCommands",
                                        css: "both",
                                        action: () => hb.Div(css: "command-right", action: () => hb
                                            .Button(
                                                controlId: "SecondaryAuthenticate",
                                                controlCss: " button-icon validate button-positive",
                                                text: Displays.Confirm(context: context),
                                                onClick: "$p.send($(this));",
                                                icon: "ui-icon-unlocked",
                                                action: "Authenticate",
                                                method: "post",
                                                type: "submit")
                                            .Button(
                                                    text: Displays.Cancel(context: context),
                                                    controlCss: "button-icon button-neutral",
                                                    onClick: "$p.back();",
                                                    icon: "ui-icon-cancel")))
                        .Div(
                            id: "TotpRegisterBottom",
                            action: () => hb.Raw(HtmlHtmls.ExtendedHtmls(
                                context: context,
                                id: "TotpRegisterBottom"))))
                .Val(
                    target: "#BackUrl",
                    value: context.UrlReferrer,
                    _using: !context.UrlReferrer.IsNullOrEmpty()).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public HtmlBuilder TotpAuthenticationCodeSeparate(HtmlBuilder hb, Context context)
        {
            return hb.FieldTextBox(
                textType: HtmlTypes.TextTypes.Normal,
                controlId: "SecondaryAuthenticationCode",
                controlCss: "always-send totp-form",
                labelText: Displays.AuthenticationCode(context: context),
                validateRequired: true)
                .Div(
                    id: "TotpAuthenticationCodeSeparate",
                    action: () => hb
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.Normal,
                            controlId: "FirstTotpAuthenticationCode",
                            controlCss: "focus totp-authentication-code",
                            validateNumber: true)
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.Normal,
                            controlCss: "totp-authentication-code",
                            validateNumber: true)
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.Normal,
                            controlCss: "totp-authentication-code",
                            validateNumber: true)
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.Normal,
                            controlCss: "totp-authentication-code",
                            validateNumber: true)
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.Normal,
                            controlCss: "totp-authentication-code",
                            validateNumber: true)
                        .FieldTextBox(
                            textType: HtmlTypes.TextTypes.Normal,
                            controlCss: "totp-authentication-code",
                            validateNumber: true)); ;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Allow(
            Context context,
            string returnUrl,
            bool atLogin = false,
            bool createPersistentCookie = false,
            bool noHttpContext = false)
        {
            context.LoginId = this.LoginId;
            string loginAfterUrl = AllowAfterUrl(
                context: context,
                returnUrl: returnUrl,
                createPersistentCookie: createPersistentCookie,
                noHttpContext: noHttpContext);
            return new UsersResponseCollection(
                context: context,
                userModel: this)
                    .CloseDialog(_using: atLogin)
                    .Message(
                        message: Messages.LoginIn(context: context),
                        target: "#LoginMessage")
                    .Href(loginAfterUrl).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string AllowAfterUrl(
            Context context,
            string returnUrl,
            bool createPersistentCookie = false,
            bool noHttpContext = false)
        {
            context.SetUserProperties(
                userModel: this,
                noHttpContext: noHttpContext);
            var loginAfterUrl = GetReturnUrl(
                context: context,
                returnUrl: returnUrl);
            IncrementsNumberOfLogins(context: context);
            SetFormsAuthentication(
                context: context,
                createPersistentCookie: createPersistentCookie);
            return loginAfterUrl.IsNullOrEmpty()
                ? Locations.Top(context: context)
                : loginAfterUrl;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void IncrementsNumberOfLogins(Context context)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(
                        context: context,
                        userModel: this),
                    param: Rds.UsersParam()
                        .NumberOfLogins(raw: "\"Users\".\"NumberOfLogins\"+1")
                        .LastLoginTime(DateTime.Now),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void IncrementsNumberOfDenial(Context context, bool disableUpdateLastLoginTime = false)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhere().Add(
                        name: "LoginId",
                        value: LoginId,
                        raw: "(lower(\"Users\".\"LoginId\") = lower(@LoginId))"),
                    param: Rds.UsersParam()
                        .NumberOfDenial(raw: "\"Users\".\"NumberOfDenial\"+1")
                        .LastLoginTime(
                            value: DateTime.Now,
                            _using: !disableUpdateLastLoginTime),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void LoginSuccessLog(Context context)
        {
            if (Parameters.SysLog.LoginSuccess || Parameters.SysLog.ClientId)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(Authenticate),
                    message: LoginMessage(
                        success: Parameters.SysLog.LoginSuccess
                            ? true
                            : null));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void LoginFailureLog(Context context, string description)
        {
            if (Parameters.SysLog.LoginFailure || Parameters.SysLog.ClientId)
            {
                new SysLogModel(
                    context: context,
                    method: nameof(Authenticate),
                    sysLogsStatus: 401,
                    sysLogsDescription: $"{Debugs.GetSysLogsDescription()}:{Messages.Authentication(context: context).Text}",
                    message: LoginMessage(
                        success: Parameters.SysLog.LoginFailure
                            ? false
                            : null));
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string LoginMessage(bool? success)
        {
            return new
            {
                LoginId = success != null ? LoginId : null,
                Success = success,
                ClientId = Parameters.SysLog.ClientId
                    ? AspNetCoreCurrentRequestContext.AspNetCoreHttpContext.Current?.Request.Cookies["Pleasanter_ClientId"]
                    : null
            }.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string Deny(Context context)
        {
            DenyLog(context: context, disableUpdateLastLoginTime: true);
            return Messages.ResponseAuthentication(
                context: context,
                target: "#LoginMessage")
                    .Focus("#Password").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string DenyLoginExpired(Context context)
        {
            DenyLog(context: context, disableUpdateLastLoginTime: true);
            return Messages.ResponseLoginExpired(
                context: context,
                target: "#LoginMessage")
                    .Focus("#Password").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void DenyLog(Context context, bool disableUpdateLastLoginTime = false)
        {
            LoginFailureLog(
                context: context,
                description: nameof(Deny));
            IncrementsNumberOfDenial(context: context, disableUpdateLastLoginTime: disableUpdateLastLoginTime);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string InvalidIpAddress(Context context)
        {
            LoginFailureLog(
                context: context,
                description: nameof(InvalidIpAddress));
            return Messages.ResponseInvalidIpAddress(
                context: context,
                target: "#LoginMessage")
                    .Focus("#Password").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string UserDisabled(Context context)
        {
            LoginFailureLog(
                context: context,
                description: nameof(UserDisabled));
            IncrementsNumberOfDenial(context: context);
            return Messages.ResponseUserDisabled(
                context: context,
                target: "#LoginMessage")
                    .Focus("#Password").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string UserLockout(Context context)
        {
            LoginFailureLog(
                context: context,
                description: nameof(UserLockout));
            return Messages.ResponseUserLockout(
                context: context,
                target: "#LoginMessage")
                    .Focus("#Password").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string OpenChangePasswordAtLoginDialog(Context context)
        {
            return new ResponseCollection(context: context)
                .Invoke("openChangePasswordDialog")
                .ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool PasswordExpired()
        {
            return
                PasswordExpirationTime.Value.InRange() &&
                PasswordExpirationTime.Value <= DateTime.Now;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool LoginExpired()
        {
            return
                (
                    LoginExpirationLimit.Value.InRange() &&
                    LoginExpirationLimit.Value <= DateTime.Now
                ) ||
                (
                    LoginExpirationPeriod != 0 &&
                    LastLoginTime.Value.AddDays(LoginExpirationPeriod).InRange() &&
                    LastLoginTime.Value.AddDays(LoginExpirationPeriod) <= DateTime.Now
                );
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool EnabledSecondaryAuthentication(Context context)
        {
            switch (Parameters.Security.SecondaryAuthentication?.Mode)
            {
                case ParameterAccessor.Parts.SecondaryAuthentication.SecondaryAuthenticationMode.None:
                    return false;
                case ParameterAccessor.Parts.SecondaryAuthentication.SecondaryAuthenticationMode.DefaultEnable:
                    if (DisableSecondaryAuthentication)
                    {
                        return false;
                    }
                    break;
                case ParameterAccessor.Parts.SecondaryAuthentication.SecondaryAuthenticationMode.DefaultDisable:
                    if (!EnableSecondaryAuthentication)
                    {
                        return false;
                    }
                    break;
                default:
                    return false;
            }
            var statements = new List<SqlStatement>().OnUseSecondaryAuthentication(context: context).ToArray();
            if (!(statements?.Any() == true))
            {
                return true;
            }
            statements.ForEach(statement => statement.SqlParamCollection = new SqlParamCollection()
                .Add("TenantId", TenantId)
                .Add("UserId", UserId));
            var dataTables = statements.Select(statement => Repository.ExecuteTable(
                context: context,
                statements: statement));
            foreach (DataTable table in dataTables)
            {
                if (table.Rows.Count == 0)
                {
                    return true;
                }
                if (!table.AsEnumerable().All(dataRow => dataRow[0]?.ToBool() == false))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool SecondaryAuthentication(Context context, string secondaryAuthenticationCode, bool isAuthenticationByMail)
        {
            return isAuthenticationByMail
                ? SecondaryAuthenticationCode == secondaryAuthenticationCode
                    && SecondaryAuthenticationCodeExpirationTime.Value.InRange()
                    && SecondaryAuthenticationCodeExpirationTime.Value > DateTime.Now
                : VerifyTotp(secondaryAuthenticationCode);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool VerifyTotp(string secondaryAuthenticationCode, double countTolerances = 0)
        {
            OtpNet.Totp totp = new OtpNet.Totp(OtpNet.Base32Encoding.ToBytes(SecretKey), totpSize: 6);
            var beforeTime = countTolerances * -30;
            return countTolerances >= Parameters.Security.SecondaryAuthentication.CountTolerances
                ? false
                : totp.VerifyTotp(DateTime.UtcNow.AddSeconds((double)beforeTime),
                    secondaryAuthenticationCode, out _,
                    OtpNet.VerificationWindow.RfcSpecifiedNetworkDelay)
                        ? true
                        : VerifyTotp(secondaryAuthenticationCode, countTolerances + 1);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetFormsAuthentication(Context context, bool createPersistentCookie)
        {
            LoginSuccessLog(context: context);
            if (context.Request)
            {
                context.FormsAuthenticationSignIn(
                    userName: LoginId,
                    createPersistentCookie: createPersistentCookie);
            }
            Libraries.Initializers.StatusesInitializer.Initialize(new Context(
                tenantId: TenantId,
                deptId: DeptId,
                userId: UserId,
                context: context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types ChangePassword(Context context)
        {
            if (Parameters.Security.EnforcePasswordHistories > 0)
            {
                if (PasswordHistries?.Any(o => o.Password == ChangedPassword) == true)
                {
                    return Error.Types.PasswordHasBeenUsed;
                }
                SetPasswordHistories(
                    context: context,
                    password: ChangedPassword);
            }
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(
                        context: context,
                        userModel: this),
                    param: ChangePasswordParam(
                        context: context,
                        password: ChangedPassword)));
            Get(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types ChangePasswordAtLogin(Context context)
        {
            if (Parameters.Security.EnforcePasswordHistories > 0)
            {
                if (PasswordHistries?.Any(o => o.Password == ChangedPassword) == true)
                {
                    return Error.Types.PasswordHasBeenUsed;
                }
                SetPasswordHistories(
                    context: context,
                    password: ChangedPassword);
            }
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(
                        context: context,
                        userModel: this),
                    param: ChangePasswordParam(
                        context: context,
                        password: ChangedPassword,
                        changeAtLogin: true)));
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types ResetPassword(Context context)
        {
            if (Parameters.Security.EnforcePasswordHistories > 0)
            {
                if (PasswordHistries?.Any(o => o.Password == AfterResetPassword) == true)
                {
                    return Error.Types.PasswordHasBeenUsed;
                }
                SetPasswordHistories(
                    context: context,
                    password: AfterResetPassword);
            }
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(
                        context: context,
                        userModel: this),
                    param: ChangePasswordParam(
                        context: context,
                        password: AfterResetPassword)));
            Get(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context));
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetPasswordHistories(Context context, string password)
        {
            if (PasswordHistries == null)
            {
                PasswordHistries = new List<PasswordHistory>();
            }
            PasswordHistries.Insert(0, new PasswordHistory()
            {
                Password = password,
                Creator = context.UserId,
                CreatedTime = DateTime.Now
            });
            PasswordHistries = PasswordHistries
                .Take(Parameters.Security.EnforcePasswordHistories)
                .ToList();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public SqlParamCollection ChangePasswordParam(
            Context context, string password, bool changeAtLogin = false)
        {
            SetPasswordExpirationPeriod(context: context);
            var param = Rds.UsersParam()
                .Password(password)
                .PasswordChangeTime(raw: $"{context.Sqls.CurrentDateTime}")
                .PasswordHistries(PasswordHistries.ToJson());
            return Parameters.Security.PasswordExpirationPeriod > 0 || !changeAtLogin
                ? param.PasswordExpirationTime(PasswordExpirationTime.Value)
                : param.PasswordExpirationTime(raw: "null");
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return UserId.ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToResponse(Context context, SiteSettings ss, Column column)
        {
            return UserId.ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToDisplay(Context context, SiteSettings ss, Column column)
        {
            return Name;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToLookup(Context context, SiteSettings ss, Column column, Lookup.Types? type)
        {
            switch (type)
            {
                case Lookup.Types.DisplayName:
                    return ToDisplay(
                        context: context,
                        ss: ss,
                        column: column);
                default:
                    return UserId.ToString();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public HtmlBuilder Td(
            HtmlBuilder hb,
            Context context,
            Column column,
            int? tabIndex,
            ServerScriptModelColumn serverScriptModelColumn)
        {
            return UserId != 0
                ? hb.Td(
                    css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                    action: () => hb
                        .HtmlUser(
                            context: context,
                            text: column.ChoiceHash.Get(UserId.ToString())?.Text))
                : hb.Td(
                    css: column.CellCss(serverScriptModelColumn?.ExtendedCellCss),
                    action: () => { });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public object ToApiDisplayValue(Context context, SiteSettings ss, Column column)
        {
            var name = SiteInfo.Name(
                context: context,
                id: UserId,
                type: Column.Types.User);
            return !name.IsNullOrEmpty()
                ? name
                : null;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public object ToApiValue(Context context, SiteSettings ss, Column column)
        {
            return UserId;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return SiteInfo.UserName(
                context: context,
                userId: UserId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types AddMailAddress(
            Context context, string mailAddress, IEnumerable<string> selected)
        {
            MailAddresses = Session_MailAddresses(context: context) ?? new List<string>();
            if (MailAddresses.Contains(mailAddress))
            {
                return Error.Types.AlreadyAdded;
            }
            else
            {
                MailAddresses.Add(mailAddress);
                Session_MailAddresses(
                    context: context,
                    value: MailAddresses.ToJson());
                return Error.Types.None;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types DeleteMailAddresses(Context context, IEnumerable<string> selected)
        {
            MailAddresses = Session_MailAddresses(context: context) ?? new List<string>();
            MailAddresses.RemoveAll(o => selected.Contains(o));
            Session_MailAddresses(
                context: context,
                value: MailAddresses.ToJson());
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ErrorData CreateApiKey(Context context, SiteSettings ss)
        {
            ApiKey = Guid.NewGuid().ToString().Sha512Cng();
            return Update(
                context: context,
                ss: ss,
                updateMailAddresses: false);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ErrorData DeleteApiKey(Context context, SiteSettings ss)
        {
            ApiKey = string.Empty;
            return Update(
                context: context,
                ss: ss,
                updateMailAddresses: false);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool InitialValue(Context context)
        {
            return UserId == 0;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void InitializeTimeZone()
        {
            if (TimeZoneInfo.GetSystemTimeZones().Any(info => info.Id == TimeZone)) return;
            TimeZone = (TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(info => info.Id == "Tokyo Standard Time" || info.Id == "Asia/Tokyo") ?? TimeZoneInfo.Local)?.Id;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string GetReturnUrl(Context context, string returnUrl)
        {
            // returnUrlがローカルURL以外の場合に、string.Emptyに詰め替える。
            if (!returnUrl.StartsWith("/") && !returnUrl.StartsWith("~/")) returnUrl = string.Empty;
            if (Permissions.PrivilegedUsers(LoginId) && Parameters.Locations.LoginAfterUrlExcludePrivilegedUsers)
            {
                return returnUrl;
            }
            if (returnUrl.IsNullOrEmpty() || returnUrl == "/")
            {
                var dashboardUrl = Locations.DashboardUrl(context: context);
                return dashboardUrl.IsNullOrEmpty()
                    ? Locations.Get(
                        context: context,
                        parts: Parameters.Locations.LoginAfterUrl)
                    : dashboardUrl;
            }
            return returnUrl;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public List<string> GetMailAddresses(Context context)
        {
            return Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectMailAddresses(
                    column: Rds.MailAddressesColumn().MailAddress(),
                    where: Rds.MailAddressesWhere()
                        .OwnerId(UserId)
                        .OwnerType("Users")))
                            .AsEnumerable()
                            .Select(dataRow => dataRow.String("MailAddress"))
                            .ToList();
        }
    }
}
