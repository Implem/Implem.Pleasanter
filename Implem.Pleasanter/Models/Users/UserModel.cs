using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public class UserModel : BaseModel, Interfaces.IConvertable
    {
        public int TenantId = Sessions.TenantId();
        public int UserId = 0;
        public string LoginId = string.Empty;
        public bool Disabled = false;
        public string UserCode = string.Empty;
        public string Password = string.Empty;
        public string PasswordValidate = string.Empty;
        public string PasswordDummy = string.Empty;
        public bool RememberMe = false;
        public string LastName = string.Empty;
        public string FirstName = string.Empty;
        public Time Birthday = null;
        public string Sex = "1";
        public string Language = "ja";
        public string TimeZone = "Tokyo Standard Time";
        public int DeptId = 0;
        public Names.FirstAndLastNameOrders FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)2;
        public Time LastLoginTime = null;
        public Time PasswordExpirationTime = null;
        public Time PasswordChangeTime = null;
        public int NumberOfLogins = 0;
        public int NumberOfDenial = 0;
        public bool TenantAdmin = false;
        public bool ServiceAdmin = false;
        public bool Developer = false;
        public string OldPassword = string.Empty;
        public string ChangedPassword = string.Empty;
        public string ChangedPasswordValidator = string.Empty;
        public string AfterResetPassword = string.Empty;
        public string AfterResetPasswordValidator = string.Empty;
        public List<string> MailAddresses = new List<string>();
        public string DemoMailAddress = string.Empty;
        public string SessionGuid = string.Empty;
        public string FullName1 { get { return FirstName + " " + LastName; } }
        public string FullName2 { get { return LastName + " " + FirstName; } }
        public TimeZoneInfo TimeZoneInfo { get { return TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(o => o.Id == TimeZone); } }
        public DeptModel Dept { get { return SiteInfo.DeptModel(DeptId); } }
        public Title Title { get { return new Title(UserId, FullName()); } }
        public int SavedTenantId = Sessions.TenantId();
        public int SavedUserId = 0;
        public string SavedLoginId = string.Empty;
        public bool SavedDisabled = false;
        public string SavedUserCode = string.Empty;
        public string SavedPassword = string.Empty;
        public string SavedPasswordValidate = string.Empty;
        public string SavedPasswordDummy = string.Empty;
        public bool SavedRememberMe = false;
        public string SavedLastName = string.Empty;
        public string SavedFirstName = string.Empty;
        public DateTime SavedBirthday = 0.ToDateTime();
        public string SavedSex = "1";
        public string SavedLanguage = "ja";
        public string SavedTimeZone = "Tokyo Standard Time";
        public int SavedDeptId = 0;
        public int SavedFirstAndLastNameOrder = 2;
        public DateTime SavedLastLoginTime = 0.ToDateTime();
        public DateTime SavedPasswordExpirationTime = 0.ToDateTime();
        public DateTime SavedPasswordChangeTime = 0.ToDateTime();
        public int SavedNumberOfLogins = 0;
        public int SavedNumberOfDenial = 0;
        public bool SavedTenantAdmin = false;
        public bool SavedServiceAdmin = false;
        public bool SavedDeveloper = false;
        public string SavedOldPassword = string.Empty;
        public string SavedChangedPassword = string.Empty;
        public string SavedChangedPasswordValidator = string.Empty;
        public string SavedAfterResetPassword = string.Empty;
        public string SavedAfterResetPasswordValidator = string.Empty;
        public string SavedMailAddresses = string.Empty;
        public string SavedDemoMailAddress = string.Empty;
        public string SavedSessionGuid = string.Empty;
        public bool TenantId_Updated { get { return TenantId != SavedTenantId; } }
        public bool UserId_Updated { get { return UserId != SavedUserId; } }
        public bool LoginId_Updated { get { return LoginId != SavedLoginId && LoginId != null; } }
        public bool Disabled_Updated { get { return Disabled != SavedDisabled; } }
        public bool UserCode_Updated { get { return UserCode != SavedUserCode && UserCode != null; } }
        public bool Password_Updated { get { return Password != SavedPassword && Password != null; } }
        public bool LastName_Updated { get { return LastName != SavedLastName && LastName != null; } }
        public bool FirstName_Updated { get { return FirstName != SavedFirstName && FirstName != null; } }
        public bool Birthday_Updated { get { return Birthday.Value != SavedBirthday && Birthday.Value != null; } }
        public bool Sex_Updated { get { return Sex != SavedSex && Sex != null; } }
        public bool Language_Updated { get { return Language != SavedLanguage && Language != null; } }
        public bool TimeZone_Updated { get { return TimeZone != SavedTimeZone && TimeZone != null; } }
        public bool DeptId_Updated { get { return DeptId != SavedDeptId; } }
        public bool FirstAndLastNameOrder_Updated { get { return FirstAndLastNameOrder.ToInt() != SavedFirstAndLastNameOrder; } }
        public bool LastLoginTime_Updated { get { return LastLoginTime.Value != SavedLastLoginTime && LastLoginTime.Value != null; } }
        public bool PasswordExpirationTime_Updated { get { return PasswordExpirationTime.Value != SavedPasswordExpirationTime && PasswordExpirationTime.Value != null; } }
        public bool PasswordChangeTime_Updated { get { return PasswordChangeTime.Value != SavedPasswordChangeTime && PasswordChangeTime.Value != null; } }
        public bool NumberOfLogins_Updated { get { return NumberOfLogins != SavedNumberOfLogins; } }
        public bool NumberOfDenial_Updated { get { return NumberOfDenial != SavedNumberOfDenial; } }
        public bool TenantAdmin_Updated { get { return TenantAdmin != SavedTenantAdmin; } }
        public bool ServiceAdmin_Updated { get { return ServiceAdmin != SavedServiceAdmin; } }
        public bool Developer_Updated { get { return Developer != SavedDeveloper; } }

        public List<string> Session_MailAddresses()
        {
            return this.PageSession("MailAddresses") != null
                ? this.PageSession("MailAddresses") as List<string>
                : MailAddresses;
        }

        public void  Session_MailAddresses(object value)
        {
            this.PageSession("MailAddresses", value);
        }

        public List<int> SwitchTargets;

        public UserModel()
        {
        }

        public UserModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            PermissionType = permissionType;
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public UserModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            int userId,
            bool clearSessions = false,
            bool setByForm = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            UserId = userId;
            PermissionType = permissionType;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public UserModel(
            SiteSettings siteSettings, 
            Permissions.Types permissionType,
            DataRow dataRow)
        {
            OnConstructing();
            SiteSettings = siteSettings;
            Set(dataRow);
            OnConstructed();
        }

        private void OnConstructing()
        {
        }

        private void OnConstructed()
        {
        }

        public void ClearSessions()
        {
            Session_MailAddresses(null);
        }

        public UserModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectUsers(
                tableType: tableType,
                column: column ?? Rds.UsersDefaultColumns(),
                join: join ??  Rds.UsersJoinDefault(),
                where: where ?? Rds.UsersWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public string Create(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            var error = ValidateBeforeCreate();
            if (error != null) return error;
            OnCreating();
            var newId = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertUsers(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.UsersParamDefault(
                            this, setDefault: true, paramAll: paramAll))
                });
            UserId = newId != 0 ? newId : UserId;
            Get();
            OnCreated();
            return RecordResponse(this, Messages.Created(Title.ToString()));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnCreating()
        {
            PasswordExpirationPeriod();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnCreated()
        {
            SetSiteInfo();
        }

        private string ValidateBeforeCreate()
        {
            if (!PermissionType.CanEditTenant() && !Self())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Users_TenantId": if (!SiteSettings.GetColumn("TenantId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_UserId": if (!SiteSettings.GetColumn("UserId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Ver": if (!SiteSettings.GetColumn("Ver").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_LoginId": if (!SiteSettings.GetColumn("LoginId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Disabled": if (!SiteSettings.GetColumn("Disabled").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_UserCode": if (!SiteSettings.GetColumn("UserCode").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Password": if (!SiteSettings.GetColumn("Password").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_PasswordValidate": if (!SiteSettings.GetColumn("PasswordValidate").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_PasswordDummy": if (!SiteSettings.GetColumn("PasswordDummy").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_RememberMe": if (!SiteSettings.GetColumn("RememberMe").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_LastName": if (!SiteSettings.GetColumn("LastName").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_FirstName": if (!SiteSettings.GetColumn("FirstName").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_FullName1": if (!SiteSettings.GetColumn("FullName1").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_FullName2": if (!SiteSettings.GetColumn("FullName2").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Birthday": if (!SiteSettings.GetColumn("Birthday").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Sex": if (!SiteSettings.GetColumn("Sex").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Language": if (!SiteSettings.GetColumn("Language").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_TimeZone": if (!SiteSettings.GetColumn("TimeZone").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_TimeZoneInfo": if (!SiteSettings.GetColumn("TimeZoneInfo").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_DeptId": if (!SiteSettings.GetColumn("DeptId").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Dept": if (!SiteSettings.GetColumn("Dept").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_FirstAndLastNameOrder": if (!SiteSettings.GetColumn("FirstAndLastNameOrder").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Title": if (!SiteSettings.GetColumn("Title").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_LastLoginTime": if (!SiteSettings.GetColumn("LastLoginTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_PasswordExpirationTime": if (!SiteSettings.GetColumn("PasswordExpirationTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_PasswordChangeTime": if (!SiteSettings.GetColumn("PasswordChangeTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_NumberOfLogins": if (!SiteSettings.GetColumn("NumberOfLogins").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_NumberOfDenial": if (!SiteSettings.GetColumn("NumberOfDenial").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_TenantAdmin": if (!SiteSettings.GetColumn("TenantAdmin").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_ServiceAdmin": if (!SiteSettings.GetColumn("ServiceAdmin").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Developer": if (!SiteSettings.GetColumn("Developer").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_OldPassword": if (!SiteSettings.GetColumn("OldPassword").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_ChangedPassword": if (!SiteSettings.GetColumn("ChangedPassword").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_ChangedPasswordValidator": if (!SiteSettings.GetColumn("ChangedPasswordValidator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_AfterResetPassword": if (!SiteSettings.GetColumn("AfterResetPassword").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_AfterResetPasswordValidator": if (!SiteSettings.GetColumn("AfterResetPasswordValidator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_MailAddresses": if (!SiteSettings.GetColumn("MailAddresses").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_DemoMailAddress": if (!SiteSettings.GetColumn("DemoMailAddress").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_SessionGuid": if (!SiteSettings.GetColumn("SessionGuid").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Comments": if (!SiteSettings.GetColumn("Comments").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Creator": if (!SiteSettings.GetColumn("Creator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Updator": if (!SiteSettings.GetColumn("Updator").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_CreatedTime": if (!SiteSettings.GetColumn("CreatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_UpdatedTime": if (!SiteSettings.GetColumn("UpdatedTime").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_VerUp": if (!SiteSettings.GetColumn("VerUp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Timestamp": if (!SiteSettings.GetColumn("Timestamp").CanCreate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        public string Update(SqlParamCollection param = null, bool paramAll = false)
        {
            var error = ValidateBeforeUpdate();
            if (error != null) return error;
            SetBySession();
            OnUpdating(ref param);
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateUsers(
                        verUp: VerUp,
                        where: Rds.UsersWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.NotZero()),
                        param: param ?? Rds.UsersParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return ResponseConflicts();
            Get();
            var responseCollection = new UsersResponseCollection(this);
            OnUpdated(ref responseCollection);
            return ResponseByUpdate(responseCollection)
                .PrependComment(Comments, VerType)
                .ToJson();
        }

        private void OnUpdating(ref SqlParamCollection param)
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnUpdated(ref UsersResponseCollection responseCollection)
        {
            UpdateMailAddresses();
            SetSiteInfo();
        }

        private string ValidateBeforeUpdate()
        {
            if (!PermissionType.CanEditTenant() && !Self())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            foreach(var controlId in Forms.Keys())
            {
                switch (controlId)
                {
                    case "Users_TenantId": if (!SiteSettings.GetColumn("TenantId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_UserId": if (!SiteSettings.GetColumn("UserId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Ver": if (!SiteSettings.GetColumn("Ver").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_LoginId": if (!SiteSettings.GetColumn("LoginId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Disabled": if (!SiteSettings.GetColumn("Disabled").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_UserCode": if (!SiteSettings.GetColumn("UserCode").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Password": if (!SiteSettings.GetColumn("Password").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_PasswordValidate": if (!SiteSettings.GetColumn("PasswordValidate").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_PasswordDummy": if (!SiteSettings.GetColumn("PasswordDummy").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_RememberMe": if (!SiteSettings.GetColumn("RememberMe").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_LastName": if (!SiteSettings.GetColumn("LastName").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_FirstName": if (!SiteSettings.GetColumn("FirstName").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_FullName1": if (!SiteSettings.GetColumn("FullName1").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_FullName2": if (!SiteSettings.GetColumn("FullName2").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Birthday": if (!SiteSettings.GetColumn("Birthday").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Sex": if (!SiteSettings.GetColumn("Sex").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Language": if (!SiteSettings.GetColumn("Language").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_TimeZone": if (!SiteSettings.GetColumn("TimeZone").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_TimeZoneInfo": if (!SiteSettings.GetColumn("TimeZoneInfo").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_DeptId": if (!SiteSettings.GetColumn("DeptId").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Dept": if (!SiteSettings.GetColumn("Dept").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_FirstAndLastNameOrder": if (!SiteSettings.GetColumn("FirstAndLastNameOrder").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Title": if (!SiteSettings.GetColumn("Title").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_LastLoginTime": if (!SiteSettings.GetColumn("LastLoginTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_PasswordExpirationTime": if (!SiteSettings.GetColumn("PasswordExpirationTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_PasswordChangeTime": if (!SiteSettings.GetColumn("PasswordChangeTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_NumberOfLogins": if (!SiteSettings.GetColumn("NumberOfLogins").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_NumberOfDenial": if (!SiteSettings.GetColumn("NumberOfDenial").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_TenantAdmin": if (!SiteSettings.GetColumn("TenantAdmin").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_ServiceAdmin": if (!SiteSettings.GetColumn("ServiceAdmin").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Developer": if (!SiteSettings.GetColumn("Developer").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_OldPassword": if (!SiteSettings.GetColumn("OldPassword").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_ChangedPassword": if (!SiteSettings.GetColumn("ChangedPassword").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_ChangedPasswordValidator": if (!SiteSettings.GetColumn("ChangedPasswordValidator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_AfterResetPassword": if (!SiteSettings.GetColumn("AfterResetPassword").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_AfterResetPasswordValidator": if (!SiteSettings.GetColumn("AfterResetPasswordValidator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_MailAddresses": if (!SiteSettings.GetColumn("MailAddresses").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_DemoMailAddress": if (!SiteSettings.GetColumn("DemoMailAddress").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_SessionGuid": if (!SiteSettings.GetColumn("SessionGuid").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Comments": if (!SiteSettings.GetColumn("Comments").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Creator": if (!SiteSettings.GetColumn("Creator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Updator": if (!SiteSettings.GetColumn("Updator").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_CreatedTime": if (!SiteSettings.GetColumn("CreatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_UpdatedTime": if (!SiteSettings.GetColumn("UpdatedTime").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_VerUp": if (!SiteSettings.GetColumn("VerUp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                    case "Users_Timestamp": if (!SiteSettings.GetColumn("Timestamp").CanUpdate(PermissionType)) return Messages.ResponseInvalidRequest().ToJson(); break;
                }
            }
            return null;
        }

        private ResponseCollection ResponseByUpdate(UsersResponseCollection responseCollection)
        {
            return responseCollection
                .Ver()
                .Timestamp()
                .Val("#VerUp", false)
                .FormResponse(this)
                .Disabled("#VerUp", false)
                .Html("#HeaderTitle", Title.Value)
                .Html("#RecordInfo", new HtmlBuilder().RecordInfo(baseModel: this, tableName: "Users"))
                .Message(Messages.Updated(Title.ToString()))
                .RemoveComment(DeleteCommentId, _using: DeleteCommentId != 0)
                .ClearFormData();
        }

        public string UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            if (!PermissionType.CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            SetBySession();
            OnUpdatingOrCreating(ref where, ref param);
            var newId = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateOrInsertUsers(
                        selectIdentity: true,
                        where: where ?? Rds.UsersWhereDefault(this),
                        param: param ?? Rds.UsersParamDefault(this, setDefault: true))
                });
            UserId = newId != 0 ? newId : UserId;
            Get();
            var responseCollection = new UsersResponseCollection(this);
            OnUpdatedOrCreated(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnUpdatingOrCreating(
            ref SqlWhereCollection where,
            ref SqlParamCollection param)
        {
        }

        private void OnUpdatedOrCreated(ref UsersResponseCollection responseCollection)
        {
        }

        public string Delete(bool redirect = true)
        {
            if (!PermissionType.CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteUsers(
                        where: Rds.UsersWhere().UserId(UserId))
                });
            Sessions.Set("Message", Messages.Deleted(Title.Value).Html);
            var responseCollection = new UsersResponseCollection(this);
            OnDeleted(ref responseCollection);
            if (redirect)
            {
                responseCollection.Href(Navigations.Index("Users"));
            }
            return responseCollection.ToJson();
        }

        private void OnDeleting()
        {
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void OnDeleted(ref UsersResponseCollection responseCollection)
        {
            if (SiteInfo.Users.Keys.Contains(UserId))
            {
                SiteInfo.Users.Remove(UserId);
                SiteInfo.SiteUserIdCollection.ForEach(data =>
                    data.Value.RemoveAll(o => o == UserId));
            }
        }

        public string Restore(int userId)
        {
            if (!Permissions.Admins().CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            UserId = userId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreUsers(
                        where: Rds.UsersWhere().UserId(UserId))
                });
            return new ResponseCollection().ToJson();
        }

        public string PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            if (!PermissionType.CanEditTenant())
            {
                return Messages.ResponseHasNotPermission().ToJson();
            }
            OnPhysicalDeleting();
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteUsers(
                    tableType: tableType,
                    param: Rds.UsersParam().UserId(UserId)));
            var responseCollection = new UsersResponseCollection(this);
            OnPhysicalDeleted(ref responseCollection);
            return responseCollection.ToJson();
        }

        private void OnPhysicalDeleting()
        {
        }

        private void OnPhysicalDeleted(ref UsersResponseCollection responseCollection)
        {
        }

        public string Histories()
        {
            var hb = new HtmlBuilder();
            hb.Table(
                attributes: new HtmlAttributes().Class("grid"),
                action: () => hb
                    .THead(action: () => hb
                        .GridHeader(
                            columnCollection: SiteSettings.HistoryColumnCollection(),
                            sort: false,
                            checkRow: false))
                    .TBody(action: () =>
                        new UserCollection(
                            siteSettings: SiteSettings,
                            permissionType: PermissionType,
                            where: Rds.UsersWhere().UserId(UserId),
                            orderBy: Rds.UsersOrderBy().Ver(SqlOrderBy.Types.desc),
                            tableType: Sqls.TableTypes.NormalAndHistory)
                                .ForEach(userModel => hb
                                    .Tr(
                                        attributes: new HtmlAttributes()
                                            .Class("grid-row history not-link")
                                            .DataAction("History")
                                            .DataMethod("post")
                                            .DataVer(userModel.Ver)
                                            .DataLatest(1, _using: userModel.Ver == Ver),
                                        action: () =>
                                            SiteSettings.HistoryColumnCollection()
                                                .ForEach(column => hb
                                                    .TdValue(column, userModel))))));
            return new UsersResponseCollection(this).Html("#FieldSetHistories", hb).ToJson();
        }

        public string History()
        {
            Get(
                where: Rds.UsersWhere()
                    .UserId(UserId)
                    .Ver(Forms.Int("Ver")),
                tableType: Sqls.TableTypes.NormalAndHistory);
            VerType =  Forms.Bool("Latest")
                ? Versions.VerTypes.Latest
                : Versions.VerTypes.History;
            SwitchTargets = UserUtilities.GetSwitchTargets(SiteSettings);
            return Editor();
        }

        public string Previous()
        {
            var switchTargets = UserUtilities.GetSwitchTargets(SiteSettings);
            var userModel = new UserModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                userId: switchTargets.Previous(UserId),
                switchTargets: switchTargets);
            return RecordResponse(userModel);
        }

        public string Next()
        {
            var switchTargets = UserUtilities.GetSwitchTargets(SiteSettings);
            var userModel = new UserModel(
                siteSettings: SiteSettings,
                permissionType: PermissionType,
                userId: switchTargets.Next(UserId),
                switchTargets: switchTargets);
            return RecordResponse(userModel);
        }

        public string Reload()
        {
            SwitchTargets = UserUtilities.GetSwitchTargets(SiteSettings);
            return RecordResponse(this, pushState: false);
        }

        private string RecordResponse(
            UserModel userModel, Message message = null, bool pushState = true)
        {
            userModel.MethodType = BaseModel.MethodTypes.Edit;
            return new UsersResponseCollection(this)
                .Invoke("clearDialogs")
                .ReplaceAll(
                    "#MainContainer",
                    userModel.AccessStatus == Databases.AccessStatuses.Selected
                        ? UserUtilities.Editor(userModel, byRest: true)
                        : UserUtilities.Editor(this, byRest: true))
                .Invoke("validateUsers")
                .Message(message)
                .PushState(
                    "Edit",
                    Navigations.Edit("Users", userModel.UserId),
                    _using: pushState)
                .ClearFormData()
                .ToJson();
        }

        private void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Users_LoginId": LoginId = Forms.Data(controlId).ToString(); break;
                    case "Users_Disabled": Disabled = Forms.Data(controlId).ToBool(); break;
                    case "Users_UserCode": UserCode = Forms.Data(controlId).ToString(); break;
                    case "Users_Password": Password = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_PasswordValidate": PasswordValidate = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_PasswordDummy": PasswordDummy = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_RememberMe": RememberMe = Forms.Data(controlId).ToBool(); break;
                    case "Users_LastName": LastName = Forms.Data(controlId).ToString(); break;
                    case "Users_FirstName": FirstName = Forms.Data(controlId).ToString(); break;
                    case "Users_Birthday": Birthday = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_Sex": Sex = Forms.Data(controlId).ToString(); break;
                    case "Users_Language": Language = Forms.Data(controlId).ToString(); break;
                    case "Users_TimeZone": TimeZone = Forms.Data(controlId).ToString(); break;
                    case "Users_DeptId": DeptId = Forms.Data(controlId).ToInt(); break;
                    case "Users_FirstAndLastNameOrder": FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)Forms.Data(controlId).ToInt(); break;
                    case "Users_LastLoginTime": LastLoginTime = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_PasswordExpirationTime": PasswordExpirationTime = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_PasswordChangeTime": PasswordChangeTime = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_NumberOfLogins": NumberOfLogins = Forms.Data(controlId).ToInt(); break;
                    case "Users_NumberOfDenial": NumberOfDenial = Forms.Data(controlId).ToInt(); break;
                    case "Users_TenantAdmin": TenantAdmin = Forms.Data(controlId).ToBool(); break;
                    case "Users_OldPassword": OldPassword = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_ChangedPassword": ChangedPassword = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_ChangedPasswordValidator": ChangedPasswordValidator = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_AfterResetPassword": AfterResetPassword = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_AfterResetPasswordValidator": AfterResetPasswordValidator = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_DemoMailAddress": DemoMailAddress = Forms.Data(controlId).ToString(); break;
                    case "Users_SessionGuid": SessionGuid = Forms.Data(controlId).ToString(); break;
                    case "Users_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments = Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default: break;
                }
            });
            if (Routes.Action() == "deletecomment")
            {
                DeleteCommentId = Forms.Data("ControlId").Split(',')._2nd().ToInt();
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
            Forms.FileKeys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    default: break;
                }
            });
        }

        private void SetBySession()
        {
            if (!Forms.HasData("Users_MailAddresses")) MailAddresses = Session_MailAddresses();
        }

        private void Set(DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(DataRow dataRow)
        {
            AccessStatus = Databases.AccessStatuses.Selected;
            foreach(DataColumn dataColumn in dataRow.Table.Columns)
            {
                var name = dataColumn.ColumnName;
                switch(name)
                {
                    case "TenantId": if (dataRow[name] != DBNull.Value) { TenantId = dataRow[name].ToInt(); SavedTenantId = TenantId; } break;
                    case "UserId": if (dataRow[name] != DBNull.Value) { UserId = dataRow[name].ToInt(); SavedUserId = UserId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "LoginId": LoginId = dataRow[name].ToString(); SavedLoginId = LoginId; break;
                    case "Disabled": Disabled = dataRow[name].ToBool(); SavedDisabled = Disabled; break;
                    case "UserCode": UserCode = dataRow[name].ToString(); SavedUserCode = UserCode; break;
                    case "Password": Password = dataRow[name].ToString(); SavedPassword = Password; break;
                    case "LastName": LastName = dataRow[name].ToString(); SavedLastName = LastName; break;
                    case "FirstName": FirstName = dataRow[name].ToString(); SavedFirstName = FirstName; break;
                    case "Birthday": Birthday = new Time(dataRow, "Birthday"); SavedBirthday = Birthday.Value; break;
                    case "Sex": Sex = dataRow[name].ToString(); SavedSex = Sex; break;
                    case "Language": Language = dataRow[name].ToString(); SavedLanguage = Language; break;
                    case "TimeZone": TimeZone = dataRow[name].ToString(); SavedTimeZone = TimeZone; break;
                    case "DeptId": DeptId = dataRow[name].ToInt(); SavedDeptId = DeptId; break;
                    case "FirstAndLastNameOrder": FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)dataRow[name].ToInt(); SavedFirstAndLastNameOrder = FirstAndLastNameOrder.ToInt(); break;
                    case "LastLoginTime": LastLoginTime = new Time(dataRow, "LastLoginTime"); SavedLastLoginTime = LastLoginTime.Value; break;
                    case "PasswordExpirationTime": PasswordExpirationTime = new Time(dataRow, "PasswordExpirationTime"); SavedPasswordExpirationTime = PasswordExpirationTime.Value; break;
                    case "PasswordChangeTime": PasswordChangeTime = new Time(dataRow, "PasswordChangeTime"); SavedPasswordChangeTime = PasswordChangeTime.Value; break;
                    case "NumberOfLogins": NumberOfLogins = dataRow[name].ToInt(); SavedNumberOfLogins = NumberOfLogins; break;
                    case "NumberOfDenial": NumberOfDenial = dataRow[name].ToInt(); SavedNumberOfDenial = NumberOfDenial; break;
                    case "TenantAdmin": TenantAdmin = dataRow[name].ToBool(); SavedTenantAdmin = TenantAdmin; break;
                    case "ServiceAdmin": ServiceAdmin = dataRow[name].ToBool(); SavedServiceAdmin = ServiceAdmin; break;
                    case "Developer": Developer = dataRow[name].ToBool(); SavedDeveloper = Developer; break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }

        private string Editor()
        {
            return new UsersResponseCollection(this)
                .ReplaceAll(
                    "#MainContainer",
                    UserUtilities.Editor(this, byRest: true))
                .Invoke("validateUsers")
                .ToJson();
        }

        private string ResponseConflicts()
        {
            Get();
            return AccessStatus == Databases.AccessStatuses.Selected
                ? Messages.ResponseUpdateConflicts(Updator.FullName()).ToJson()
                : Messages.ResponseDeleteConflicts().ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void UpdateMailAddresses()
        {
            var statements = new List<SqlStatement>
            {
                Rds.DeleteMailAddresses(
                    where: Rds.MailAddressesWhere()
                        .OwnerId(UserId)
                        .OwnerType("Users"))
            };
            Session_MailAddresses().ForEach(mailAddress =>
                statements.Add(Rds.InsertMailAddresses(
                    param: Rds.MailAddressesParam()
                        .OwnerId(UserId)
                        .OwnerType("Users")
                        .MailAddress(mailAddress))));
            Rds.ExecuteNonQuery(transactional: true, statements: statements.ToArray());
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void SetSiteInfo()
        {
            SiteInfo.SiteUserIdCollection.Clear();
            SiteInfo.SetUser(User());
            if (Self()) SetSession();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private void PasswordExpirationPeriod()
        {
            PasswordExpirationTime = Parameters.Authentication.PasswordExpirationPeriod != 0
                ? new Time(DateTime.Today.AddDays(
                    Parameters.Authentication.PasswordExpirationPeriod))
                : new Time();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public User User()
        {
            return new User()
            {
                Id = UserId,
                DeptId = DeptId,
                FirstName = FirstName,
                LastName = LastName,
                FirstAndLastNameOrders = FirstAndLastNameOrder,
                TenantAdmin = TenantAdmin,
                ServiceAdmin = ServiceAdmin
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public RdsUser RdsUser()
        {
            return new RdsUser()
            {
                UserId = UserId,
                DeptId = DeptId
            };
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public UserModel(RdsUser.UserTypes userType)
        {
            OnConstructing();
            UserId = userType.ToInt();
            Get();
            OnConstructed();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public UserModel(string loginId)
        {
            SetByForm();
            Get(where: Rds.UsersWhere().LoginId(loginId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool Self()
        {
            return Sessions.UserId() == UserId;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string FullName()
        {
            return Names.FullName(FirstAndLastNameOrder, FullName1, FullName2);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Authenticate(string returnUrl)
        {
            if (Authenticate())
            {
                if (PasswordExpired())
                {
                    return OpenChangePasswordAtLoginDialog();
                }
                else
                {
                    return Allow(returnUrl);
                }
            }
            else
            {
                return Deny();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool Authenticate()
        {
            var ret = false;
            switch (Parameters.Authentication.Provider)
            {
                case "LDAP":
                    ret = Ldap.Authenticate();
                    if (ret) Get(where: Rds.UsersWhere().LoginId(LoginId));
                    break;
                default:
                    ret = GetByCredentials();
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private bool GetByCredentials()
        {
            Get(where: Rds.UsersWhere()
                .LoginId(LoginId)
                .Password(Password)
                .Disabled(0));
            return AccessStatus == Databases.AccessStatuses.Selected;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string Allow(string returnUrl, bool atLogin = false)
        {
            Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                where: Rds.UsersWhereDefault(this),
                param: Rds.UsersParam()
                    .NumberOfLogins(raw: "[NumberOfLogins] + 1")
                    .LastLoginTime(DateTime.Now)));
            SetFormsAuthentication(returnUrl);
            return new UsersResponseCollection(this)
                .CloseDialog("#ChangePasswordDialog", _using: atLogin)
                .Message(Messages.LoginIn())
                .Href(returnUrl == string.Empty
                    ? Navigations.Top()
                    : returnUrl).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string Deny()
        {
            Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                param: Rds.UsersParam()
                    .NumberOfDenial(raw: "[NumberOfDenial] + 1"),
                where: Rds.UsersWhere()
                    .LoginId(LoginId)));
            return Messages.ResponseAuthentication().Focus("#Password").ToJson();
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
                PasswordExpirationTime.Value.NotZero() &&
                PasswordExpirationTime.Value <= DateTime.Now;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetFormsAuthentication(string returnUrl)
        {
            System.Web.Security.FormsAuthentication.SetAuthCookie(
                UserId.ToString(), Forms.Bool("Users_RememberMe"));
            SetSession();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSession()
        {
            HttpContext.Session["TenantId"] = TenantId;
            HttpContext.Session["UserId"] = UserId;
            HttpContext.Session["Language"] = Language;
            HttpContext.Session["Developer"] = Developer;
            HttpContext.Session["TimeZoneInfo"] = TimeZoneInfo;
            HttpContext.Session["RdsUser"] = RdsUser();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ChangePassword()
        {
            var responseCollection = new UsersResponseCollection(this);
            if (UserId == Sessions.UserId())
            {
                if (OldPassword == ChangedPassword)
                {
                    return Messages.ResponsePasswordNotChanged().ToJson();
                }
                if (GetByCredentials())
                {
                    PasswordExpirationPeriod();
                    Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                        where: Rds.UsersWhereDefault(this),
                        param: Rds.UsersParam()
                            .Password(ChangedPassword)
                            .PasswordExpirationTime(PasswordExpirationTime.Value)
                            .PasswordChangeTime(raw: "getdate()")));
                    responseCollection
                        .PasswordExpirationTime(PasswordExpirationTime.ToString())
                        .PasswordChangeTime(PasswordChangeTime.ToString())
                        .UpdatedTime(UpdatedTime.ToString())
                        .OldPassword(string.Empty)
                        .ChangedPassword(string.Empty)
                        .ChangedPasswordValidator(string.Empty)
                        .ClearFormData()
                        .CloseDialog()
                        .Message(Messages.ChangingPasswordComplete());
                }
                else
                {
                    responseCollection.Message(Messages.IncorrectCurrentPassword());
                }
            }
            else
            {
                responseCollection.Message(Messages.HasNotPermission());
            }
            return responseCollection.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ChangePasswordAtLogin()
        {
            if (OldPassword == ChangedPassword)
            {
                return Messages.ResponsePasswordNotChanged().ToJson();
            }
            if (GetByCredentials())
            {
                PasswordExpirationPeriod();
                Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(this),
                    param: Rds.UsersParam()
                        .Password(ChangedPassword)
                        .PasswordExpirationTime(PasswordExpirationTime.Value)
                        .PasswordChangeTime(raw: "getdate()")));
                return Allow(Forms.Data("ReturnUrl"), atLogin: true);
            }
            else
            {
                return Messages.ResponseIncorrectCurrentPassword().ToJson();
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ResetPassword()
        {
            var responseCollection = new UsersResponseCollection(this);
            var siteSettings = SiteSettingsUtility.UsersSiteSettings();
            if (Permissions.Admins().CanEditTenant())
            {
                PasswordExpirationPeriod();
                Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                    where: Rds.UsersWhereDefault(this),
                    param: Rds.UsersParam()
                        .Password(AfterResetPassword)
                        .PasswordExpirationTime(PasswordExpirationTime.Value)
                        .PasswordChangeTime(raw: "getdate()")));
                responseCollection
                    .PasswordExpirationTime(PasswordExpirationTime.ToString())
                    .PasswordChangeTime(PasswordChangeTime.ToString())
                    .UpdatedTime(UpdatedTime.ToString())
                    .AfterResetPassword(string.Empty)
                    .AfterResetPasswordValidator(string.Empty)
                    .ClearFormData()
                    .CloseDialog()
                    .Message(Messages.PasswordResetCompleted());
            }
            else
            {
                responseCollection.Message(Messages.HasNotPermission());
            }
            return responseCollection.ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToControl(Column column, Permissions.Types permissionType)
        {
            return UserId.ToString();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToResponse()
        {
            return string.Empty;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public HtmlBuilder Td(HtmlBuilder hb, Column column)
        {
            return UserId != 0 ?
                hb.Td(action: () => hb
                    .HtmlUser(UserId)) :
                hb.Td(action: () => { });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToExport(Column column)
        {
            return SiteInfo.UserFullName(UserId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string AddMailAddress()
        {
            var mailAddress = Forms.Data("MailAddress").Trim();
            var selected = Forms.Data("MailAddresses").Split(';');
            var mailAddresses = Session_MailAddresses();
            var badMailAddress = Libraries.Mails.Addresses.BadAddress(mailAddress);
            if (badMailAddress != string.Empty)
            {
                return Messages
                    .ResponseBadMailAddress(badMailAddress)
                    .Focus("#MailAddress")
                    .ToJson();
            }
            if (!mailAddresses.Contains(mailAddress))
            {
                mailAddresses.Add(mailAddress);
                Session_MailAddresses(mailAddresses);
            }
            return ResponseMailAddresses(selected, mailAddresses);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string DeleteMailAddresses()
        {
            var selected = Forms.Data("MailAddresses").Split(';');
            var mailAddresses = Session_MailAddresses();
            mailAddresses.RemoveAll(o => selected.Contains(o));
            Session_MailAddresses(mailAddresses);
            return ResponseMailAddresses(selected, mailAddresses);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private static string ResponseMailAddresses(string[] selected, List<string> mailAddresses)
        {
            return new ResponseCollection()
                .Html(
                    "#MailAddresses",
                    new HtmlBuilder().SelectableItems(
                        listItemCollection: mailAddresses.ToDictionary(o => o, o => o),
                        selectedValueTextCollection: selected))
                .Val("#MailAddress", string.Empty)
                .Focus("#MailAddress")
                .ToJson();
        }
    }
}
