using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.DataTypes;
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
using System.Linq;
namespace Implem.Pleasanter.Models
{
    public class UserModel : BaseModel, Interfaces.IConvertable
    {
        public int TenantId = Sessions.TenantId();
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
        public bool Developer = false;
        public UserSettings UserSettings = new UserSettings();
        public string OldPassword = string.Empty;
        public string ChangedPassword = string.Empty;
        public string ChangedPasswordValidator = string.Empty;
        public string AfterResetPassword = string.Empty;
        public string AfterResetPasswordValidator = string.Empty;
        public List<string> MailAddresses = new List<string>();
        public string DemoMailAddress = string.Empty;
        public string SessionGuid = string.Empty;
        public TimeZoneInfo TimeZoneInfo { get { return TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(o => o.Id == TimeZone); } }
        public Dept Dept { get { return SiteInfo.Dept(DeptId); } }
        public Title Title { get { return new Title(UserId, Name); } }
        public int SavedTenantId = Sessions.TenantId();
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
        public bool SavedDeveloper = false;
        public string SavedUserSettings = string.Empty;
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
        public bool GlobalId_Updated { get { return GlobalId != SavedGlobalId && GlobalId != null; } }
        public bool Name_Updated { get { return Name != SavedName && Name != null; } }
        public bool UserCode_Updated { get { return UserCode != SavedUserCode && UserCode != null; } }
        public bool Password_Updated { get { return Password != SavedPassword && Password != null; } }
        public bool LastName_Updated { get { return LastName != SavedLastName && LastName != null; } }
        public bool FirstName_Updated { get { return FirstName != SavedFirstName && FirstName != null; } }
        public bool Birthday_Updated { get { return Birthday.Value != SavedBirthday && Birthday.Value != null; } }
        public bool Gender_Updated { get { return Gender != SavedGender && Gender != null; } }
        public bool Language_Updated { get { return Language != SavedLanguage && Language != null; } }
        public bool TimeZone_Updated { get { return TimeZone != SavedTimeZone && TimeZone != null; } }
        public bool DeptId_Updated { get { return DeptId != SavedDeptId; } }
        public bool FirstAndLastNameOrder_Updated { get { return FirstAndLastNameOrder.ToInt() != SavedFirstAndLastNameOrder; } }
        public bool Body_Updated { get { return Body != SavedBody && Body != null; } }
        public bool LastLoginTime_Updated { get { return LastLoginTime.Value != SavedLastLoginTime && LastLoginTime.Value != null; } }
        public bool PasswordExpirationTime_Updated { get { return PasswordExpirationTime.Value != SavedPasswordExpirationTime && PasswordExpirationTime.Value != null; } }
        public bool PasswordChangeTime_Updated { get { return PasswordChangeTime.Value != SavedPasswordChangeTime && PasswordChangeTime.Value != null; } }
        public bool NumberOfLogins_Updated { get { return NumberOfLogins != SavedNumberOfLogins; } }
        public bool NumberOfDenial_Updated { get { return NumberOfDenial != SavedNumberOfDenial; } }
        public bool TenantManager_Updated { get { return TenantManager != SavedTenantManager; } }
        public bool ServiceManager_Updated { get { return ServiceManager != SavedServiceManager; } }
        public bool Disabled_Updated { get { return Disabled != SavedDisabled; } }
        public bool Developer_Updated { get { return Developer != SavedDeveloper; } }
        public bool UserSettings_Updated { get { return UserSettings.RecordingJson() != SavedUserSettings && UserSettings.RecordingJson() != null; } }

        public UserSettings Session_UserSettings()
        {
            return this.PageSession("UserSettings") != null
                ? this.PageSession("UserSettings")?.ToString().Deserialize<UserSettings>() ?? new UserSettings()
                : UserSettings;
        }

        public void  Session_UserSettings(object value)
        {
            this.PageSession("UserSettings", value);
        }

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
            SiteSettings ss, 
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            if (setByForm) SetByForm(ss);
            MethodType = methodType;
            OnConstructed();
        }

        public UserModel(
            SiteSettings ss, 
            int userId,
            bool clearSessions = false,
            bool setByForm = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            UserId = userId;
            Get(ss);
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm(ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public UserModel(SiteSettings ss, DataRow dataRow)
        {
            OnConstructing();
            Set(ss, dataRow);
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
            Session_UserSettings(null);
            Session_MailAddresses(null);
        }

        public UserModel Get(
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
            Set(ss, Rds.ExecuteTable(statements: Rds.SelectUsers(
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

        public Error.Types Create(
            SiteSettings ss, 
            RdsUser rdsUser = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false,
            bool get = true)
        {
            PasswordExpirationPeriod();
            var statements = CreateStatements(ss, tableType, param, paramAll);
            try
            {
                var newId = Rds.ExecuteScalar_int(
                    transactional: true, statements: statements.ToArray());
                UserId = newId != 0 ? newId : UserId;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (e.Number == 2601)
                {
                    return Error.Types.LoginIdAlreadyUse;
                }
                else
                {
                    throw;
                }
            }
            if (get) Get(ss);
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            SiteSettings ss, 
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            return new List<SqlStatement>
            {
                Rds.InsertUsers(
                    tableType: tableType,
                        selectIdentity: true,
                    param: param ?? Rds.UsersParamDefault(
                        this, setDefault: true, paramAll: paramAll)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.UsersUpdated)
            };
        }

        public Error.Types Update(
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            RdsUser rdsUser = null,
            SqlParamCollection param = null,
            bool paramAll = false,
            bool get = true)
        {
            SetBySession();
            var statements = UpdateStatements(param, paramAll);
            try
            {
                var count = Rds.ExecuteScalar_int(
                    rdsUser: rdsUser,
                    transactional: true,
                    statements: statements.ToArray());
                if (count == 0) return Error.Types.UpdateConflicts;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                if (e.Number == 2601)
                {
                    return Error.Types.LoginIdAlreadyUse;
                }
                else
                {
                    throw;
                }
            }
            if (get) Get(ss);
            UpdateMailAddresses();
            SetSiteInfo();
            return Error.Types.None;
        }

        public List<SqlStatement> UpdateStatements(
            SqlParamCollection param, bool paramAll = false)
        {
            var timestamp = Timestamp.ToDateTime();
            return new List<SqlStatement>
            {
                Rds.UpdateUsers(
                    verUp: VerUp,
                    where: Rds.UsersWhereDefault(this)
                        .UpdatedTime(timestamp, _using: timestamp.InRange()),
                    param: param ?? Rds.UsersParamDefault(this, paramAll: paramAll),
                    countRecord: true),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.UsersUpdated)
            };
        }

        public Error.Types Delete(SiteSettings ss, bool notice = false)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteUsers(
                        where: Rds.UsersWhere().UserId(UserId)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.UsersUpdated)
                });
            var userHash = SiteInfo.TenantCaches[Sessions.TenantId()].UserHash;
            if (userHash.Keys.Contains(UserId))
            {
                userHash.Remove(UserId);
            }
            return Error.Types.None;
        }

        public Error.Types Restore(SiteSettings ss, int userId)
        {
            UserId = userId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreUsers(
                        where: Rds.UsersWhere().UserId(UserId)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.UsersUpdated)
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            SiteSettings ss, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteUsers(
                    tableType: tableType,
                    param: Rds.UsersParam().UserId(UserId)));
            return Error.Types.None;
        }

        public void SetByForm(SiteSettings ss)
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Users_LoginId": LoginId = Forms.Data(controlId).ToString(); break;
                    case "Users_GlobalId": GlobalId = Forms.Data(controlId).ToString(); break;
                    case "Users_Name": Name = Forms.Data(controlId).ToString(); break;
                    case "Users_UserCode": UserCode = Forms.Data(controlId).ToString(); break;
                    case "Users_Password": Password = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_PasswordValidate": PasswordValidate = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_PasswordDummy": PasswordDummy = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_RememberMe": RememberMe = Forms.Data(controlId).ToBool(); break;
                    case "Users_LastName": LastName = Forms.Data(controlId).ToString(); break;
                    case "Users_FirstName": FirstName = Forms.Data(controlId).ToString(); break;
                    case "Users_Birthday": Birthday = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_Gender": Gender = Forms.Data(controlId).ToString(); break;
                    case "Users_Language": Language = Forms.Data(controlId).ToString(); break;
                    case "Users_TimeZone": TimeZone = Forms.Data(controlId).ToString(); break;
                    case "Users_DeptId": DeptId = Forms.Data(controlId).ToInt(); break;
                    case "Users_FirstAndLastNameOrder": FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)Forms.Data(controlId).ToInt(); break;
                    case "Users_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Users_LastLoginTime": LastLoginTime = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_PasswordExpirationTime": PasswordExpirationTime = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_PasswordChangeTime": PasswordChangeTime = new Time(Forms.Data(controlId).ToDateTime(), byForm: true); break;
                    case "Users_NumberOfLogins": NumberOfLogins = Forms.Data(controlId).ToInt(); break;
                    case "Users_NumberOfDenial": NumberOfDenial = Forms.Data(controlId).ToInt(); break;
                    case "Users_TenantManager": TenantManager = Forms.Data(controlId).ToBool(); break;
                    case "Users_Disabled": Disabled = Forms.Data(controlId).ToBool(); break;
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
                DeleteCommentId = Forms.ControlId().Split(',')._2nd().ToInt();
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
            if (!Forms.HasData("Users_UserSettings")) UserSettings = Session_UserSettings();
            if (!Forms.HasData("Users_MailAddresses")) MailAddresses = Session_MailAddresses();
        }

        private void Set(SiteSettings ss, DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(SiteSettings ss, DataRow dataRow)
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
                    case "GlobalId": GlobalId = dataRow[name].ToString(); SavedGlobalId = GlobalId; break;
                    case "Name": Name = dataRow[name].ToString(); SavedName = Name; break;
                    case "UserCode": UserCode = dataRow[name].ToString(); SavedUserCode = UserCode; break;
                    case "Password": Password = dataRow[name].ToString(); SavedPassword = Password; break;
                    case "LastName": LastName = dataRow[name].ToString(); SavedLastName = LastName; break;
                    case "FirstName": FirstName = dataRow[name].ToString(); SavedFirstName = FirstName; break;
                    case "Birthday": Birthday = new Time(dataRow, "Birthday"); SavedBirthday = Birthday.Value; break;
                    case "Gender": Gender = dataRow[name].ToString(); SavedGender = Gender; break;
                    case "Language": Language = dataRow[name].ToString(); SavedLanguage = Language; break;
                    case "TimeZone": TimeZone = dataRow[name].ToString(); SavedTimeZone = TimeZone; break;
                    case "DeptId": DeptId = dataRow[name].ToInt(); SavedDeptId = DeptId; break;
                    case "FirstAndLastNameOrder": FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)dataRow[name].ToInt(); SavedFirstAndLastNameOrder = FirstAndLastNameOrder.ToInt(); break;
                    case "Body": Body = dataRow[name].ToString(); SavedBody = Body; break;
                    case "LastLoginTime": LastLoginTime = new Time(dataRow, "LastLoginTime"); SavedLastLoginTime = LastLoginTime.Value; break;
                    case "PasswordExpirationTime": PasswordExpirationTime = new Time(dataRow, "PasswordExpirationTime"); SavedPasswordExpirationTime = PasswordExpirationTime.Value; break;
                    case "PasswordChangeTime": PasswordChangeTime = new Time(dataRow, "PasswordChangeTime"); SavedPasswordChangeTime = PasswordChangeTime.Value; break;
                    case "NumberOfLogins": NumberOfLogins = dataRow[name].ToInt(); SavedNumberOfLogins = NumberOfLogins; break;
                    case "NumberOfDenial": NumberOfDenial = dataRow[name].ToInt(); SavedNumberOfDenial = NumberOfDenial; break;
                    case "TenantManager": TenantManager = dataRow[name].ToBool(); SavedTenantManager = TenantManager; break;
                    case "ServiceManager": ServiceManager = dataRow[name].ToBool(); SavedServiceManager = ServiceManager; break;
                    case "Disabled": Disabled = dataRow[name].ToBool(); SavedDisabled = Disabled; break;
                    case "Developer": Developer = dataRow[name].ToBool(); SavedDeveloper = Developer; break;
                    case "UserSettings": UserSettings = GetUserSettings(dataRow); SavedUserSettings = UserSettings.RecordingJson(); break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }

        public bool Updated()
        {
            return
                TenantId_Updated ||
                UserId_Updated ||
                Ver_Updated ||
                LoginId_Updated ||
                GlobalId_Updated ||
                Name_Updated ||
                UserCode_Updated ||
                Password_Updated ||
                LastName_Updated ||
                FirstName_Updated ||
                Birthday_Updated ||
                Gender_Updated ||
                Language_Updated ||
                TimeZone_Updated ||
                DeptId_Updated ||
                FirstAndLastNameOrder_Updated ||
                Body_Updated ||
                LastLoginTime_Updated ||
                PasswordExpirationTime_Updated ||
                PasswordChangeTime_Updated ||
                NumberOfLogins_Updated ||
                NumberOfDenial_Updated ||
                TenantManager_Updated ||
                ServiceManager_Updated ||
                Disabled_Updated ||
                Developer_Updated ||
                UserSettings_Updated ||
                Comments_Updated ||
                Creator_Updated ||
                Updator_Updated ||
                CreatedTime_Updated ||
                UpdatedTime_Updated;
        }

        public List<string> Mine()
        {
            var mine = new List<string>();
            var userId = Sessions.UserId();
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
        private void UpdateMailAddresses()
        {
            var statements = new List<SqlStatement>
            {
                Rds.DeleteMailAddresses(
                    where: Rds.MailAddressesWhere()
                        .OwnerId(UserId)
                        .OwnerType("Users"))
            };
            Session_MailAddresses()?.ForEach(mailAddress =>
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
            SiteInfo.Reflesh();
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
            Get(SiteSettingsUtilities.UsersSiteSettings());
            OnConstructed();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public UserModel(string loginId)
        {
            var ss = SiteSettingsUtilities.UsersSiteSettings();
            SetByForm(ss);
            Get(ss, where: Rds.UsersWhere().LoginId(loginId));
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
                var tenantOptions = TenantOptions();
                if (tenantOptions?.Any() == true)
                {
                    return TenantsDropDown(tenantOptions);
                }
                else
                {
                    return Deny();
                }
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool Authenticate()
        {
            var ret = false;
            switch (Parameters.Authentication.Provider)
            {
                case "LDAP":
                    ret = Ldap.Authenticate(LoginId, Forms.Data("Users_Password"));
                    if (ret)
                    {
                        Get(SiteSettingsUtilities.UsersSiteSettings(),
                            where: Rds.UsersWhere().LoginId(LoginId));
                    }
                    break;
                case "Extension":
                    var user = Extension.Authenticate(LoginId, Password);
                    ret = user != null;
                    if (ret)
                    {
                        Get(SiteSettingsUtilities.UsersSiteSettings(),
                            where: Rds.UsersWhere()
                                .TenantId(user.TenantId)
                                .UserId(user.Id));
                    }
                    break;
                default:
                    ret = GetByCredentials(LoginId, Password, Forms.Int("SelectedTenantId"));
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool GetByCredentials(string loginId, string password, int tenantId = 0)
        {
            Get(SiteSettingsUtilities.UsersSiteSettings(),
                where: Rds.UsersWhere()
                    .LoginId(loginId)
                    .Password(password)
                    .Disabled(0));
            return AccessStatus == Databases.AccessStatuses.Selected;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private string TenantsDropDown(Dictionary<string, string> tenantOptions)
        {
            return new ResponseCollection()
                .Html("#Tenants", new HtmlBuilder().FieldDropDown(
                    controlId: "SelectedTenantId",
                    fieldCss: " field-wide",
                    controlCss: " always-send",
                    labelText: Displays.Tenants(),
                    optionCollection: tenantOptions)).ToJson();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        private Dictionary<string, string> TenantOptions()
        {
            return Rds.ExecuteScalar_string(statements:
                Rds.SelectLoginKeys(
                    column: Rds.LoginKeysColumn().TenantNames(),
                    where: Rds.LoginKeysWhere()
                        .LoginId(LoginId)))
                            .Deserialize<Dictionary<string, string>>();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string Allow(string returnUrl, bool atLogin = false)
        {
            Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                where: Rds.UsersWhereDefault(this),
                param: Rds.UsersParam()
                    .NumberOfLogins(raw: "[NumberOfLogins] + 1")
                    .LastLoginTime(DateTime.Now)));
            SetFormsAuthentication(returnUrl);
            return new UsersResponseCollection(this)
                .CloseDialog(_using: atLogin)
                .Message(Messages.LoginIn())
                .Href(returnUrl == string.Empty
                    ? Locations.Top()
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
                PasswordExpirationTime.Value.InRange() &&
                PasswordExpirationTime.Value <= DateTime.Now;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetFormsAuthentication(string returnUrl)
        {
            System.Web.Security.FormsAuthentication.SetAuthCookie(
                UserId.ToString(), Forms.Bool("Users_RememberMe"));
            Sessions.SetTenantId(TenantId);
            SetSession();
            Libraries.Initializers.StatusesInitializer.Initialize(TenantId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void SetSession()
        {
            HttpContext.Session["UserId"] = UserId;
            HttpContext.Session["Language"] = Language;
            HttpContext.Session["Developer"] = Developer;
            HttpContext.Session["TimeZoneInfo"] = TimeZoneInfo;
            HttpContext.Session["RdsUser"] = RdsUser();
            HttpContext.Session["UserSettings"] = UserSettings.ToJson();
            Contract.Set();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types ChangePassword()
        {
            PasswordExpirationPeriod();
            Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                where: Rds.UsersWhereDefault(this),
                param: Rds.UsersParam()
                    .Password(ChangedPassword)
                    .PasswordExpirationTime(PasswordExpirationTime.Value)
                    .PasswordChangeTime(raw: "getdate()")));
            Get(SiteSettingsUtilities.UsersSiteSettings());
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types ChangePasswordAtLogin()
        {
            PasswordExpirationPeriod();
            Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                where: Rds.UsersWhereDefault(this),
                param: Rds.UsersParam()
                    .Password(ChangedPassword)
                    .PasswordExpirationTime(PasswordExpirationTime.Value)
                    .PasswordChangeTime(raw: "getdate()")));
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types ResetPassword()
        {
            PasswordExpirationPeriod();
            Rds.ExecuteNonQuery(statements: Rds.UpdateUsers(
                where: Rds.UsersWhereDefault(this),
                param: Rds.UsersParam()
                    .Password(AfterResetPassword)
                    .PasswordExpirationTime(PasswordExpirationTime.Value)
                    .PasswordChangeTime(raw: "getdate()")));
            Get(SiteSettingsUtilities.UsersSiteSettings());
            return Error.Types.None;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToControl(SiteSettings ss, Column column)
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
        public string ToExport(Column column, ExportColumn exportColumn)
        {
            return SiteInfo.UserName(UserId);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types AddMailAddress(string mailAddress, IEnumerable<string> selected)
        {
            MailAddresses = Session_MailAddresses() ?? new List<string>();
            if (MailAddresses.Contains(mailAddress))
            {
                return Error.Types.AlreadyAdded;
            }
            else
            { 
                MailAddresses.Add(mailAddress);
                Session_MailAddresses(MailAddresses);
                return Error.Types.None;
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types DeleteMailAddresses(IEnumerable<string> selected)
        {
            MailAddresses = Session_MailAddresses() ?? new List<string>();
            MailAddresses.RemoveAll(o => selected.Contains(o));
            Session_MailAddresses(MailAddresses);
            return Error.Types.None;
        }
    }
}
