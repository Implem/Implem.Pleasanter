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
using Implem.Pleasanter.Libraries.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
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
        public string Language = "ja";
        public string TimeZone = "Tokyo Standard Time";
        public string DeptCode = string.Empty;
        public int DeptId = 0;
        public Names.FirstAndLastNameOrders FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)2;
        public string Body = string.Empty;
        public Time LastLoginTime = new Time();
        public Time PasswordExpirationTime = new Time();
        public Time PasswordChangeTime = new Time();
        public int NumberOfLogins = 0;
        public int NumberOfDenial = 0;
        public bool TenantManager = false;
        public bool ServiceManager = false;
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
        public List<string> MailAddresses = new List<string>();
        public string DemoMailAddress = string.Empty;
        public string SessionGuid = string.Empty;
        public string LdapSearchRoot = string.Empty;
        public DateTime SynchronizedTime = 0.ToDateTime();

        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                return TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(o => o.Id == TimeZone) ?? TimeZoneInfo.Local;
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
        public string SavedLanguage = "ja";
        public string SavedTimeZone = "Tokyo Standard Time";
        public string SavedDeptCode = string.Empty;
        public int SavedDeptId = 0;
        public int SavedFirstAndLastNameOrder = 2;
        public string SavedBody = string.Empty;
        public DateTime SavedLastLoginTime = 0.ToDateTime();
        public DateTime SavedPasswordExpirationTime = 0.ToDateTime();
        public DateTime SavedPasswordChangeTime = 0.ToDateTime();
        public int SavedNumberOfLogins = 0;
        public int SavedNumberOfDenial = 0;
        public bool SavedTenantManager = false;
        public bool SavedServiceManager = false;
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
        public string SavedMailAddresses = string.Empty;
        public string SavedDemoMailAddress = string.Empty;
        public string SavedSessionGuid = string.Empty;
        public string SavedLdapSearchRoot = string.Empty;
        public DateTime SavedSynchronizedTime = 0.ToDateTime();

        public bool TenantId_Updated(Context context, Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool UserId_Updated(Context context, Column column = null)
        {
            return UserId != SavedUserId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != UserId);
        }

        public bool LoginId_Updated(Context context, Column column = null)
        {
            return LoginId != SavedLoginId && LoginId != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != LoginId);
        }

        public bool GlobalId_Updated(Context context, Column column = null)
        {
            return GlobalId != SavedGlobalId && GlobalId != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != GlobalId);
        }

        public bool Name_Updated(Context context, Column column = null)
        {
            return Name != SavedName && Name != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Name);
        }

        public bool UserCode_Updated(Context context, Column column = null)
        {
            return UserCode != SavedUserCode && UserCode != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != UserCode);
        }

        public bool Password_Updated(Context context, Column column = null)
        {
            return Password != SavedPassword && Password != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Password);
        }

        public bool LastName_Updated(Context context, Column column = null)
        {
            return LastName != SavedLastName && LastName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != LastName);
        }

        public bool FirstName_Updated(Context context, Column column = null)
        {
            return FirstName != SavedFirstName && FirstName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != FirstName);
        }

        public bool Gender_Updated(Context context, Column column = null)
        {
            return Gender != SavedGender && Gender != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Gender);
        }

        public bool Language_Updated(Context context, Column column = null)
        {
            return Language != SavedLanguage && Language != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Language);
        }

        public bool TimeZone_Updated(Context context, Column column = null)
        {
            return TimeZone != SavedTimeZone && TimeZone != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != TimeZone);
        }

        public bool DeptId_Updated(Context context, Column column = null)
        {
            return DeptId != SavedDeptId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != DeptId);
        }

        public bool FirstAndLastNameOrder_Updated(Context context, Column column = null)
        {
            return FirstAndLastNameOrder.ToInt() != SavedFirstAndLastNameOrder &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != FirstAndLastNameOrder.ToInt());
        }

        public bool Body_Updated(Context context, Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Body);
        }

        public bool NumberOfLogins_Updated(Context context, Column column = null)
        {
            return NumberOfLogins != SavedNumberOfLogins &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != NumberOfLogins);
        }

        public bool NumberOfDenial_Updated(Context context, Column column = null)
        {
            return NumberOfDenial != SavedNumberOfDenial &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != NumberOfDenial);
        }

        public bool TenantManager_Updated(Context context, Column column = null)
        {
            return TenantManager != SavedTenantManager &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != TenantManager);
        }

        public bool ServiceManager_Updated(Context context, Column column = null)
        {
            return ServiceManager != SavedServiceManager &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != ServiceManager);
        }

        public bool Disabled_Updated(Context context, Column column = null)
        {
            return Disabled != SavedDisabled &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != Disabled);
        }

        public bool Lockout_Updated(Context context, Column column = null)
        {
            return Lockout != SavedLockout &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != Lockout);
        }

        public bool LockoutCounter_Updated(Context context, Column column = null)
        {
            return LockoutCounter != SavedLockoutCounter &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != LockoutCounter);
        }

        public bool Developer_Updated(Context context, Column column = null)
        {
            return Developer != SavedDeveloper &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToBool() != Developer);
        }

        public bool UserSettings_Updated(Context context, Column column = null)
        {
            return UserSettings.RecordingJson() != SavedUserSettings && UserSettings.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != UserSettings.RecordingJson());
        }

        public bool ApiKey_Updated(Context context, Column column = null)
        {
            return ApiKey != SavedApiKey && ApiKey != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != ApiKey);
        }

        public bool LdapSearchRoot_Updated(Context context, Column column = null)
        {
            return LdapSearchRoot != SavedLdapSearchRoot && LdapSearchRoot != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != LdapSearchRoot);
        }

        public bool Birthday_Updated(Context context, Column column = null)
        {
            return Birthday.Value != SavedBirthday &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != Birthday.Value.Date);
        }

        public bool LastLoginTime_Updated(Context context, Column column = null)
        {
            return LastLoginTime.Value != SavedLastLoginTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != LastLoginTime.Value.Date);
        }

        public bool PasswordExpirationTime_Updated(Context context, Column column = null)
        {
            return PasswordExpirationTime.Value != SavedPasswordExpirationTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != PasswordExpirationTime.Value.Date);
        }

        public bool PasswordChangeTime_Updated(Context context, Column column = null)
        {
            return PasswordChangeTime.Value != SavedPasswordChangeTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != PasswordChangeTime.Value.Date);
        }

        public bool SynchronizedTime_Updated(Context context, Column column = null)
        {
            return SynchronizedTime != SavedSynchronizedTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != SynchronizedTime.Date);
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
            List<string> mine)
        {
            var value = string.Empty;
            switch (column.Name)
            {
                case "TenantId":
                    value = ss.ReadColumnAccessControls.Allowed(
                        context: context,
                        ss: ss,
                        column: column,
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
                        mine: mine)
                            ? TimeZone.ToExport(
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
                        mine: mine)
                            ? Dept.ToExport(
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
                        mine: mine)
                            ? ServiceManager.ToExport(
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
                        mine: mine)
                            ? ApiKey.ToExport(
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
                        mine: mine)
                            ? SynchronizedTime.ToExport(
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
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
                        type: ss.PermissionType,
                        mine: mine)
                            ? UpdatedTime.ToExport(
                                context: context,
                                column: column,
                                exportColumn: exportColumn)
                            : string.Empty;
                    break;
                default:
                    switch (Def.ExtendedColumnTypes.Get(column.Name))
                    {
                        case "Class":
                            value = ss.ReadColumnAccessControls.Allowed(
                                context: context,
                                ss: ss,
                                column: column,
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Class(columnName: column.Name).ToExport(
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
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Num(columnName: column.Name).ToExport(
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
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Date(columnName: column.Name).ToExport(
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
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Description(columnName: column.Name).ToExport(
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
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Check(columnName: column.Name).ToExport(
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
                                type: ss.PermissionType,
                                mine: mine)
                                    ? Attachments(columnName: column.Name).ToExport(
                                        context: context,
                                        column: column,
                                        exportColumn: exportColumn)
                                    : string.Empty;
                            break;
                        default: return string.Empty;
                    }
                    break;
            }
            return "\"" + value?.Replace("\"", "\"\"") + "\"";
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
            IDictionary<string, string> formData = null,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (setByApi) SetByApi(context: context, ss: ss);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public UserModel(
            Context context,
            SiteSettings ss,
            int userId,
            IDictionary<string, string> formData = null,
            bool setByApi = false,
            bool clearSessions = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            UserId = userId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    where: Rds.UsersWhereDefault(this)
                        .Users_Ver(context.QueryStrings.Int("ver")), ss: ss);
            }
            else
            {
                Get(context: context, ss: ss);
            }
            if (clearSessions) ClearSessions(context: context);
            if (formData != null)
            {
                SetByForm(
                    context: context,
                    ss: ss,
                    formData: formData);
            }
            if (setByApi) SetByApi(context: context, ss: ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public UserModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            IDictionary<string, string> formData = null,
            string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
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
        /// <param name="context"></param>
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
            Set(context, ss, Repository.ExecuteTable(
                context: context,
                statements: Rds.SelectUsers(
                    tableType: tableType,
                    column: column ?? Rds.UsersDefaultColumns(),
                    join: join ??  Rds.UsersJoinDefault(),
                    where: where ?? Rds.UsersWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public UserApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new UserApiModel()
            {
                ApiVersion = context.ApiVersion
            };
            ss.ReadableColumns(noJoined: true).ForEach(column =>
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
                    case "FirstAndLastNameOrder": data.FirstAndLastNameOrder = FirstAndLastNameOrder.ToInt(); break;
                    case "Body": data.Body = Body; break;
                    case "LastLoginTime": data.LastLoginTime = LastLoginTime.Value.ToLocal(context: context); break;
                    case "PasswordExpirationTime": data.PasswordExpirationTime = PasswordExpirationTime.Value.ToLocal(context: context); break;
                    case "PasswordChangeTime": data.PasswordChangeTime = PasswordChangeTime.Value.ToLocal(context: context); break;
                    case "NumberOfLogins": data.NumberOfLogins = NumberOfLogins; break;
                    case "NumberOfDenial": data.NumberOfDenial = NumberOfDenial; break;
                    case "TenantManager": data.TenantManager = TenantManager; break;
                    case "ServiceManager": data.ServiceManager = ServiceManager; break;
                    case "Disabled": data.Disabled = Disabled; break;
                    case "Lockout": data.Lockout = Lockout; break;
                    case "LockoutCounter": data.LockoutCounter = LockoutCounter; break;
                    case "Developer": data.Developer = Developer; break;
                    case "UserSettings": data.UserSettings = UserSettings.RecordingJson(); break;
                    case "LdapSearchRoot": data.LdapSearchRoot = LdapSearchRoot; break;
                    case "SynchronizedTime": data.SynchronizedTime = SynchronizedTime.ToLocal(context: context); break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                    default: 
                        data.Value(
                            context: context,
                            columnName: column.ColumnName,
                            value: Value(
                                context: context,
                                column: column,
                                toLocal: true));
                        break;
                }
            });
            return data;
        }

        public ErrorData Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            bool get = true)
        {
            TenantId = context.TenantId;
            PasswordExpirationPeriod(context: context);
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
                        userModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.UsersUpdated),
            });
            statements.AddRange(UpdateAttachmentsStatements(context: context));
            return statements;
        }

        public ErrorData Update(
            Context context,
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            bool updateMailAddresses = true,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool setBySession = true,
            bool get = true)
        {
            if (setBySession)
            {
                SetBySession(context: context);
            }
            var statements = new List<SqlStatement>();
            statements.AddRange(UpdateStatements(
                context: context,
                ss: ss,
                permissions: permissions,
                permissionChanged: permissionChanged,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements));
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
                UpdateMailAddresses(context: context);
            }
            SetSiteInfo(context: context);
            return new ErrorData(type: Error.Types.None);
        }

        public List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
            string dataTableName = null,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            var where = Rds.UsersWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (Versions.VerUp(
                context: context,
                ss: ss,
                verUp: VerUp))
            {
                statements.Add(Rds.UsersCopyToStatement(
                    where: where,
                    tableType: Sqls.TableTypes.History,
                    ColumnNames()));
                Ver++;
            }
            statements.AddRange(UpdateStatements(
                context: context,
                dataTableName: dataTableName,
                where: where,
                param: param,
                otherInitValue: otherInitValue));
            statements.AddRange(UpdateAttachmentsStatements(context: context));
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
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
                        userModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(UserId)) {
                    IfConflicted = true,
                    Id = UserId
                },
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.UsersUpdated),
            };
        }

        private List<SqlStatement> UpdateAttachmentsStatements(Context context)
        {
            var statements = new List<SqlStatement>();
            ColumnNames()
                .Where(columnName => columnName.StartsWith("Attachments"))
                .Where(columnName => Attachments_Updated(columnName: columnName))
                .ForEach(columnName =>
                    Attachments(columnName: columnName).Write(
                        context: context,
                        statements: statements,
                        referenceId: UserId));
            return statements;
        }

        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.UsersWhere().UserId(UserId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteUsers(factory: context, where: where),
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
                    param: Rds.UsersParam().UserId(UserId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByForm(
            Context context,
            SiteSettings ss,
            IDictionary<string, string> formData)
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
                    case "Users_FirstAndLastNameOrder": FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)value.ToInt(); break;
                    case "Users_Body": Body = value.ToString(); break;
                    case "Users_LastLoginTime": LastLoginTime = new Time(context, value.ToDateTime(), byForm: true); break;
                    case "Users_PasswordExpirationTime": PasswordExpirationTime = new Time(context, value.ToDateTime(), byForm: true); break;
                    case "Users_PasswordChangeTime": PasswordChangeTime = new Time(context, value.ToDateTime(), byForm: true); break;
                    case "Users_NumberOfLogins": NumberOfLogins = value.ToInt(); break;
                    case "Users_NumberOfDenial": NumberOfDenial = value.ToInt(); break;
                    case "Users_TenantManager": TenantManager = value.ToBool(); break;
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
                    case "Users_LdapSearchRoot": LdapSearchRoot = value.ToString(); break;
                    case "Users_SynchronizedTime": SynchronizedTime = value.ToDateTime().ToUniversal(context: context); break;
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
                            switch (Def.ExtendedColumnTypes.Get(column?.ColumnName))
                            {
                                case "Class":
                                    Class(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Num":
                                    Num(
                                        columnName: column.ColumnName,
                                        value: column.Round(value.ToDecimal(
                                            cultureInfo: context.CultureInfo())));
                                    break;
                                case "Date":
                                    Date(
                                        columnName: column.ColumnName,
                                        value: value.ToDateTime().ToUniversal(context: context));
                                    break;
                                case "Description":
                                    Description(
                                        columnName: column.ColumnName,
                                        value: value);
                                    break;
                                case "Check":
                                    Check(
                                        columnName: column.ColumnName,
                                        value: value.ToBool());
                                    break;
                                case "Attachments":
                                    Attachments(
                                        columnName: column.ColumnName,
                                        value: value.Deserialize<Attachments>());
                                    break;
                            }
                        }
                        break;
                }
            });
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
            FirstAndLastNameOrder = userModel.FirstAndLastNameOrder;
            Body = userModel.Body;
            LastLoginTime = userModel.LastLoginTime;
            PasswordExpirationTime = userModel.PasswordExpirationTime;
            PasswordChangeTime = userModel.PasswordChangeTime;
            NumberOfLogins = userModel.NumberOfLogins;
            NumberOfDenial = userModel.NumberOfDenial;
            TenantManager = userModel.TenantManager;
            ServiceManager = userModel.ServiceManager;
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
            MailAddresses = userModel.MailAddresses;
            DemoMailAddress = userModel.DemoMailAddress;
            SessionGuid = userModel.SessionGuid;
            LdapSearchRoot = userModel.LdapSearchRoot;
            SynchronizedTime = userModel.SynchronizedTime;
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

        public void SetByApi(Context context, SiteSettings ss)
        {
            var data = context.RequestDataString.Deserialize<UserApiModel>();
            if (data == null)
            {
                return;
            }
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
            if (data.FirstAndLastNameOrder != null) FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)data.FirstAndLastNameOrder.ToInt().ToInt();
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.LastLoginTime != null) LastLoginTime = new Time(context, data.LastLoginTime.ToDateTime(), byForm: true);
            if (data.PasswordExpirationTime != null) PasswordExpirationTime = new Time(context, data.PasswordExpirationTime.ToDateTime(), byForm: true);
            if (data.PasswordChangeTime != null) PasswordChangeTime = new Time(context, data.PasswordChangeTime.ToDateTime(), byForm: true);
            if (data.NumberOfLogins != null) NumberOfLogins = data.NumberOfLogins.ToInt().ToInt();
            if (data.NumberOfDenial != null) NumberOfDenial = data.NumberOfDenial.ToInt().ToInt();
            if (data.TenantManager != null) TenantManager = data.TenantManager.ToBool().ToBool();
            if (data.Disabled != null) Disabled = data.Disabled.ToBool().ToBool();
            if (data.Lockout != null) Lockout = data.Lockout.ToBool().ToBool();
            if (data.LockoutCounter != null) LockoutCounter = data.LockoutCounter.ToInt().ToInt();
            if (data.LdapSearchRoot != null) LdapSearchRoot = data.LdapSearchRoot.ToString().ToString();
            if (data.SynchronizedTime != null) SynchronizedTime = data.SynchronizedTime.ToDateTime().ToDateTime().ToUniversal(context: context);
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
            data.ClassHash?.ForEach(o => Class(
                columnName: o.Key,
                value: o.Value));
            data.NumHash?.ForEach(o => Num(
                columnName: o.Key,
                value: o.Value));
            data.DateHash?.ForEach(o => Date(
                columnName: o.Key,
                value: o.Value.ToUniversal(context: context)));
            data.DescriptionHash?.ForEach(o => Description(
                columnName: o.Key,
                value: o.Value));
            data.CheckHash?.ForEach(o => Check(
                columnName: o.Key,
                value: o.Value));
            data.AttachmentsHash?.ForEach(o =>
            {
                string columnName = o.Key;
                Attachments newAttachments = o.Value;
                Attachments oldAttachments = AttachmentsHash.Get(columnName);
                if (oldAttachments != null)
                {
                    var newGuidSet = new HashSet<string>(newAttachments.Select(x => x.Guid).Distinct());
                    newAttachments.AddRange(oldAttachments.Where((oldvalue) => !newGuidSet.Contains(oldvalue.Guid)));
                }
                Attachments(columnName: columnName, value: newAttachments);
            });
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
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
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
                        case "LdapSearchRoot":
                            LdapSearchRoot = dataRow[column.ColumnName].ToString();
                            SavedLdapSearchRoot = LdapSearchRoot;
                            break;
                        case "SynchronizedTime":
                            SynchronizedTime = dataRow[column.ColumnName].ToDateTime();
                            SavedSynchronizedTime = SynchronizedTime;
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
                            switch (Def.ExtendedColumnTypes.Get(column.Name))
                            {
                                case "Class":
                                    Class(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedClass(
                                        columnName: column.Name,
                                        value: Class(columnName: column.Name));
                                    break;
                                case "Num":
                                    Num(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDecimal());
                                    SavedNum(
                                        columnName: column.Name,
                                        value: Num(columnName: column.Name));
                                    break;
                                case "Date":
                                    Date(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToDateTime());
                                    SavedDate(
                                        columnName: column.Name,
                                        value: Date(columnName: column.Name));
                                    break;
                                case "Description":
                                    Description(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString());
                                    SavedDescription(
                                        columnName: column.Name,
                                        value: Description(columnName: column.Name));
                                    break;
                                case "Check":
                                    Check(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToBool());
                                    SavedCheck(
                                        columnName: column.Name,
                                        value: Check(columnName: column.Name));
                                    break;
                                case "Attachments":
                                    Attachments(
                                        columnName: column.Name,
                                        value: dataRow[column.ColumnName].ToString()
                                            .Deserialize<Attachments>() ?? new Attachments());
                                    SavedAttachments(
                                        columnName: column.Name,
                                        value: Attachments(columnName: column.Name).ToJson());
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
                || FirstAndLastNameOrder_Updated(context: context)
                || Body_Updated(context: context)
                || LastLoginTime_Updated(context: context)
                || PasswordExpirationTime_Updated(context: context)
                || PasswordChangeTime_Updated(context: context)
                || NumberOfLogins_Updated(context: context)
                || NumberOfDenial_Updated(context: context)
                || TenantManager_Updated(context: context)
                || ServiceManager_Updated(context: context)
                || Disabled_Updated(context: context)
                || Lockout_Updated(context: context)
                || LockoutCounter_Updated(context: context)
                || Developer_Updated(context: context)
                || UserSettings_Updated(context: context)
                || ApiKey_Updated(context: context)
                || LdapSearchRoot_Updated(context: context)
                || SynchronizedTime_Updated(context: context)
                || Comments_Updated(context: context)
                || Creator_Updated(context: context)
                || Updator_Updated(context: context);
        }

        public List<string> Mine(Context context)
        {
            var mine = new List<string>();
            var userId = context.UserId;
            if (SavedCreator == userId) mine.Add("Creator");
            if (SavedUpdator == userId) mine.Add("Updator");
            return mine;
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

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSiteInfo(Context context)
        {
            SiteInfo.Reflesh(context: context);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void PasswordExpirationPeriod(Context context)
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
            IDictionary<string, string> formData = null)
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
        public string Authenticate(Context context, string returnUrl)
        {
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
                if (!AllowedIpAddress(context))
                {
                    return InvalidIpAddress(context: context);
                }
                else if (Lockout)
                {
                    return UserLockout(context: context);
                }
                else if(PasswordExpired())
                {
                    return OpenChangePasswordAtLoginDialog();
                }
                else 
                {
                    return Allow(
                        context: context,
                        returnUrl: returnUrl,
                        createPersistentCookie: context.Forms.Bool("Users_RememberMe"));
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
            var ret = false;
            switch (Parameters.Authentication.Provider)
            {
                case "LDAP":
                    ret = Ldap.Authenticate(
                        context: context,
                        loginId: LoginId,
                        password: context.Forms.Data("Users_Password"));
                    if (ret)
                    {
                        Get(
                            context: context,
                            ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                            where: Rds.UsersWhere().LoginId(LoginId));
                    }
                    break;
                case "LDAP+Local":
                    ret = Ldap.Authenticate(
                        context: context,
                        loginId: LoginId,
                        password: context.Forms.Data("Users_Password"));
                    if (ret)
                    {
                        Get(
                            context: context,
                            ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                            where: Rds.UsersWhere().LoginId(LoginId));
                    }
                    else
                    {
                        ret = GetByCredentials(
                            context: context,
                            loginId: LoginId,
                            password: Password,
                            tenantId: context.Forms.Int("SelectedTenantId"));
                    }
                    break;
                case "Extension":
                    ret = Extension.Authenticate(
                        context: context,
                        loginId: LoginId,
                        password: Password,
                        userModel: this);
                    break;
                case "SAML":
                    ret = GetByCredentials(
                        context: context,
                        loginId: LoginId,
                        password: Password,
                        tenantId: context.Forms.Int("SelectedTenantId"));
                    if (ret)
                    {
                        context = context.CreateContext(tenantId: TenantId);
                        if (context.ContractSettings?.AllowOriginalLogin == 0 && TenantManager == false)
                        {
                            ret = false;
                        }
                    }
                    break;
                default:
                    ret = GetByCredentials(
                        context: context,
                        loginId: LoginId,
                        password: Password,
                        tenantId: context.Forms.Int("SelectedTenantId"));
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool AllowedIpAddress(Context context)
        {
            var createdContext = context.CreateContext(TenantId);
            return context.ContractSettings.AllowedIpAddress(createdContext.UserHostAddress);
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
            Context context, string loginId, string password, int tenantId = 0)
        {
            var loginIdRaw = "(lower(\"Users\".\"LoginId\") = lower(@LoginId))";
            Get(
                context: context,
                ss: SiteSettingsUtilities.UsersSiteSettings(context: context),
                where: Rds.UsersWhere()
                    .Add(
                        name: "LoginId",
                        value: loginId,
                        raw: loginIdRaw)
                    .Password(password)
                    .Disabled(false));
            if (Parameters.Security.LockoutCount > 0)
            {
                if (AccessStatus == Databases.AccessStatuses.Selected)
                {
                    if (!Lockout)
                    {
                        Repository.ExecuteNonQuery(
                            context: context,
                            statements: Rds.UpdateUsers(
                                where: Rds.UsersWhere()
                                    .Add(
                                        name: "LoginId",
                                        value: loginId,
                                        raw: loginIdRaw),
                                param: Rds.UsersParam().LockoutCounter(0),
                                addUpdatorParam: false,
                                addUpdatedTimeParam: false));
                    }
                }
                else
                {
                    Repository.ExecuteNonQuery(
                        context: context,
                        statements: Rds.UpdateUsers(
                            where: Rds.UsersWhere()
                                .Add(
                                    name: "LoginId",
                                    value: loginId,
                                    raw: loginIdRaw),
                            param: Rds.UsersParam()
                                .Lockout(
                                    raw: "case when \"Users\".\"LockoutCounter\"+1>={0} then {1} else {2} end"
                                        .Params(
                                            Parameters.Security.LockoutCount,
                                            context.Sqls.TrueString,
                                            context.Sqls.FalseString))
                                .LockoutCounter(raw: "\"Users\".\"LockoutCounter\"+1"),
                            addUpdatorParam: false,
                            addUpdatedTimeParam: false));
                }
            }
            return AccessStatus == Databases.AccessStatuses.Selected;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string TenantsDropDown(
            Context context, Dictionary<string, string> tenantOptions)
        {
            return new ResponseCollection()
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
        public string Allow(
            Context context,
            string returnUrl,
            bool atLogin = false,
            bool createPersistentCookie = false)
        {
            IncrementsNumberOfLogins(context: context);
            SetFormsAuthentication(
                context: context,
                returnUrl: returnUrl,
                createPersistentCookie: createPersistentCookie);
            return new UsersResponseCollection(this)
                .CloseDialog(_using: atLogin)
                .Message(Messages.LoginIn(context: context))
                .Href(returnUrl == string.Empty
                    ? Locations.Top(context: context)
                    : returnUrl).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void IncrementsNumberOfLogins(Context context)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(this),
                    param: Rds.UsersParam()
                        .NumberOfLogins(raw: "\"Users\".\"NumberOfLogins\"+1")
                        .LastLoginTime(DateTime.Now),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void IncrementsNumberOfDenial(Context context)
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
                        .LastLoginTime(DateTime.Now),
                    addUpdatorParam: false,
                    addUpdatedTimeParam: false));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string Deny(Context context)
        {
            IncrementsNumberOfDenial(context: context);
            return Messages.ResponseAuthentication(context: context)
                .Focus("#Password").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string InvalidIpAddress(Context context)
        {
            return Messages.ResponseInvalidIpAddress(context: context)
                .Focus("#Password").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string UserDisabled(Context context)
        {
            IncrementsNumberOfDenial(context: context);
            return Messages.ResponseUserDisabled(context: context)
                .Focus("#Password").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string UserLockout(Context context)
        {
            return Messages.ResponseUserLockout(context: context)
                .Focus("#Password").ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string OpenChangePasswordAtLoginDialog()
        {
            return new ResponseCollection()
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
        public void SetFormsAuthentication(
            Context context, string returnUrl, bool createPersistentCookie)
        {
            context.FormsAuthenticationSignIn(
                userName: LoginId,
                createPersistentCookie: createPersistentCookie);
            Libraries.Initializers.StatusesInitializer.Initialize(context.CreateContext(
                tenantId: TenantId,
                deptId: DeptId,
                userId: UserId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types ChangePassword(Context context)
        {
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(this),
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
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(this),
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
            Repository.ExecuteNonQuery(
                context: context,
                statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(this),
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
        public SqlParamCollection ChangePasswordParam(
            Context context, string password, bool changeAtLogin = false)
        {
            PasswordExpirationPeriod(context: context);
            var param = Rds.UsersParam()
                .Password(password)
                .PasswordChangeTime(raw: $"{context.Sqls.CurrentDateTime}");
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
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public HtmlBuilder Td(HtmlBuilder hb, Context context, Column column)
        {
            return UserId != 0
                ? hb.Td(action: () => hb
                    .HtmlUser(
                        context: context,
                        text: column.ChoiceHash.Get(UserId.ToString())?.Text))
                : hb.Td(action: () => { });
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
            return Update(context: context, ss: ss);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public ErrorData DeleteApiKey(Context context, SiteSettings ss)
        {
            ApiKey = string.Empty;
            return Update(context: context, ss: ss);
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
        public System.Web.Mvc.ContentResult GetByApi(Context context)
        {
            return UserUtilities.GetByApi(
                context: context,
                ss: SiteSettingsUtilities.ApiUsersSiteSettings(context));
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
        public System.Web.Mvc.ContentResult CreateByApi(Context context)
        {
            return UserUtilities.CreateByApi(
                context: context,
                ss: SiteSettingsUtilities.ApiUsersSiteSettings(context));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public System.Web.Mvc.ContentResult UpdateByApi(Context context, int userId)
        {
            return UserUtilities.UpdateByApi(
                context: context,
                ss: SiteSettingsUtilities.ApiUsersSiteSettings(context),
                userId: userId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public System.Web.Mvc.ContentResult DeleteByApi(Context context, int userId)
        {
            return UserUtilities.DeleteByApi(
                context: context,
                ss: SiteSettingsUtilities.ApiUsersSiteSettings(context),
                userId: userId);
        }
    }
}
