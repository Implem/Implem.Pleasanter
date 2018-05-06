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
using System.Linq;
namespace Implem.Pleasanter.Models
{
    [Serializable]
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
        public string ApiKey = string.Empty;
        public string OldPassword = string.Empty;
        public string ChangedPassword = string.Empty;
        public string ChangedPasswordValidator = string.Empty;
        public string AfterResetPassword = string.Empty;
        public string AfterResetPasswordValidator = string.Empty;
        public List<string> MailAddresses = new List<string>();
        public string DemoMailAddress = string.Empty;
        public string SessionGuid = string.Empty;

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
                return SiteInfo.Dept(DeptId);
            }
        }

        public Title Title
        {
            get
            {
                return new Title(UserId, Name);
            }
        }

        [NonSerialized] public int SavedTenantId = Sessions.TenantId();
        [NonSerialized] public int SavedUserId = 0;
        [NonSerialized] public string SavedLoginId = string.Empty;
        [NonSerialized] public string SavedGlobalId = string.Empty;
        [NonSerialized] public string SavedName = string.Empty;
        [NonSerialized] public string SavedUserCode = string.Empty;
        [NonSerialized] public string SavedPassword = string.Empty;
        [NonSerialized] public string SavedPasswordValidate = string.Empty;
        [NonSerialized] public string SavedPasswordDummy = string.Empty;
        [NonSerialized] public bool SavedRememberMe = false;
        [NonSerialized] public string SavedLastName = string.Empty;
        [NonSerialized] public string SavedFirstName = string.Empty;
        [NonSerialized] public DateTime SavedBirthday = 0.ToDateTime();
        [NonSerialized] public string SavedGender = string.Empty;
        [NonSerialized] public string SavedLanguage = "ja";
        [NonSerialized] public string SavedTimeZone = "Tokyo Standard Time";
        [NonSerialized] public int SavedDeptId = 0;
        [NonSerialized] public int SavedFirstAndLastNameOrder = 2;
        [NonSerialized] public string SavedBody = string.Empty;
        [NonSerialized] public DateTime SavedLastLoginTime = 0.ToDateTime();
        [NonSerialized] public DateTime SavedPasswordExpirationTime = 0.ToDateTime();
        [NonSerialized] public DateTime SavedPasswordChangeTime = 0.ToDateTime();
        [NonSerialized] public int SavedNumberOfLogins = 0;
        [NonSerialized] public int SavedNumberOfDenial = 0;
        [NonSerialized] public bool SavedTenantManager = false;
        [NonSerialized] public bool SavedServiceManager = false;
        [NonSerialized] public bool SavedDisabled = false;
        [NonSerialized] public bool SavedDeveloper = false;
        [NonSerialized] public string SavedUserSettings = "[]";
        [NonSerialized] public string SavedApiKey = string.Empty;
        [NonSerialized] public string SavedOldPassword = string.Empty;
        [NonSerialized] public string SavedChangedPassword = string.Empty;
        [NonSerialized] public string SavedChangedPasswordValidator = string.Empty;
        [NonSerialized] public string SavedAfterResetPassword = string.Empty;
        [NonSerialized] public string SavedAfterResetPasswordValidator = string.Empty;
        [NonSerialized] public string SavedMailAddresses = string.Empty;
        [NonSerialized] public string SavedDemoMailAddress = string.Empty;
        [NonSerialized] public string SavedSessionGuid = string.Empty;

        public bool TenantId_Updated(Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != TenantId);
        }

        public bool UserId_Updated(Column column = null)
        {
            return UserId != SavedUserId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != UserId);
        }

        public bool LoginId_Updated(Column column = null)
        {
            return LoginId != SavedLoginId && LoginId != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != LoginId);
        }

        public bool GlobalId_Updated(Column column = null)
        {
            return GlobalId != SavedGlobalId && GlobalId != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != GlobalId);
        }

        public bool Name_Updated(Column column = null)
        {
            return Name != SavedName && Name != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Name);
        }

        public bool UserCode_Updated(Column column = null)
        {
            return UserCode != SavedUserCode && UserCode != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != UserCode);
        }

        public bool Password_Updated(Column column = null)
        {
            return Password != SavedPassword && Password != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Password);
        }

        public bool LastName_Updated(Column column = null)
        {
            return LastName != SavedLastName && LastName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != LastName);
        }

        public bool FirstName_Updated(Column column = null)
        {
            return FirstName != SavedFirstName && FirstName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != FirstName);
        }

        public bool Gender_Updated(Column column = null)
        {
            return Gender != SavedGender && Gender != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Gender);
        }

        public bool Language_Updated(Column column = null)
        {
            return Language != SavedLanguage && Language != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Language);
        }

        public bool TimeZone_Updated(Column column = null)
        {
            return TimeZone != SavedTimeZone && TimeZone != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != TimeZone);
        }

        public bool DeptId_Updated(Column column = null)
        {
            return DeptId != SavedDeptId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != DeptId);
        }

        public bool FirstAndLastNameOrder_Updated(Column column = null)
        {
            return FirstAndLastNameOrder.ToInt() != SavedFirstAndLastNameOrder &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != FirstAndLastNameOrder.ToInt());
        }

        public bool Body_Updated(Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Body);
        }

        public bool NumberOfLogins_Updated(Column column = null)
        {
            return NumberOfLogins != SavedNumberOfLogins &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != NumberOfLogins);
        }

        public bool NumberOfDenial_Updated(Column column = null)
        {
            return NumberOfDenial != SavedNumberOfDenial &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != NumberOfDenial);
        }

        public bool TenantManager_Updated(Column column = null)
        {
            return TenantManager != SavedTenantManager &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != TenantManager);
        }

        public bool ServiceManager_Updated(Column column = null)
        {
            return ServiceManager != SavedServiceManager &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != ServiceManager);
        }

        public bool Disabled_Updated(Column column = null)
        {
            return Disabled != SavedDisabled &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != Disabled);
        }

        public bool Developer_Updated(Column column = null)
        {
            return Developer != SavedDeveloper &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToBool() != Developer);
        }

        public bool UserSettings_Updated(Column column = null)
        {
            return UserSettings.RecordingJson() != SavedUserSettings && UserSettings.RecordingJson() != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != UserSettings.RecordingJson());
        }

        public bool ApiKey_Updated(Column column = null)
        {
            return ApiKey != SavedApiKey && ApiKey != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != ApiKey);
        }

        public bool Birthday_Updated(Column column = null)
        {
            return Birthday.Value != SavedBirthday &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != Birthday.Value.Date);
        }

        public bool LastLoginTime_Updated(Column column = null)
        {
            return LastLoginTime.Value != SavedLastLoginTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != LastLoginTime.Value.Date);
        }

        public bool PasswordExpirationTime_Updated(Column column = null)
        {
            return PasswordExpirationTime.Value != SavedPasswordExpirationTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != PasswordExpirationTime.Value.Date);
        }

        public bool PasswordChangeTime_Updated(Column column = null)
        {
            return PasswordChangeTime.Value != SavedPasswordChangeTime &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultTime().Date != PasswordChangeTime.Value.Date);
        }

        public UserSettings Session_UserSettings()
        {
            return this.PageSession("UserSettings") != null
                ? this.PageSession("UserSettings")?.ToString().Deserialize<UserSettings>() ?? new UserSettings()
                : UserSettings;
        }

        public void Session_UserSettings(object value)
        {
            this.PageSession("UserSettings", value);
        }

        public List<string> Session_MailAddresses()
        {
            return this.PageSession("MailAddresses") != null
                ? this.PageSession("MailAddresses") as List<string>
                : MailAddresses;
        }

        public void Session_MailAddresses(object value)
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
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            if (setByForm) SetByForm(ss);
            if (setByApi) SetByApi(ss);
            MethodType = methodType;
            OnConstructed();
        }

        public UserModel(
            SiteSettings ss,
            int userId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            UserId = userId;
            Get(ss);
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm(ss);
            if (setByApi) SetByApi(ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public UserModel(
            SiteSettings ss,
            DataRow dataRow,
            string tableAlias = null)
        {
            OnConstructing();
            Set(ss, dataRow, tableAlias);
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
                orderBy: orderBy,
                param: param,
                distinct: distinct,
                top: top)));
            return this;
        }

        public UserApiModel GetByApi(SiteSettings ss)
        {
            var data = new UserApiModel();
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
                    case "Birthday": data.Birthday = Birthday.Value.ToLocal(); break;
                    case "Gender": data.Gender = Gender; break;
                    case "Language": data.Language = Language; break;
                    case "TimeZone": data.TimeZone = TimeZone; break;
                    case "DeptId": data.DeptId = DeptId; break;
                    case "FirstAndLastNameOrder": data.FirstAndLastNameOrder = FirstAndLastNameOrder.ToInt(); break;
                    case "Body": data.Body = Body; break;
                    case "LastLoginTime": data.LastLoginTime = LastLoginTime.Value.ToLocal(); break;
                    case "PasswordExpirationTime": data.PasswordExpirationTime = PasswordExpirationTime.Value.ToLocal(); break;
                    case "PasswordChangeTime": data.PasswordChangeTime = PasswordChangeTime.Value.ToLocal(); break;
                    case "NumberOfLogins": data.NumberOfLogins = NumberOfLogins; break;
                    case "NumberOfDenial": data.NumberOfDenial = NumberOfDenial; break;
                    case "TenantManager": data.TenantManager = TenantManager; break;
                    case "ServiceManager": data.ServiceManager = ServiceManager; break;
                    case "Disabled": data.Disabled = Disabled; break;
                    case "Developer": data.Developer = Developer; break;
                    case "UserSettings": data.UserSettings = UserSettings.RecordingJson(); break;
                    case "ApiKey": data.ApiKey = ApiKey; break;
                    case "Creator": data.Creator = Creator.Id; break;
                    case "Updator": data.Updator = Updator.Id; break;
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(); break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(); break;
                    case "Comments": data.Comments = Comments.ToLocal().ToJson(); break;
                }
            });
            return data;
        }

        public Error.Types Create(
            SiteSettings ss,
            RdsUser rdsUser = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            bool get = true)
        {
            PasswordExpirationPeriod();
            var statements = new List<SqlStatement>();
            CreateStatements(statements, ss, tableType, param, otherInitValue);
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
            List<SqlStatement> statements,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertUsers(
                    tableType: tableType,
                        selectIdentity: true,
                    param: param ?? Rds.UsersParamDefault(
                        this, setDefault: true, otherInitValue: otherInitValue)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.UsersUpdated)
            });
            return statements;
        }

        public Error.Types Update(
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
            RdsUser rdsUser = null,
            SqlParamCollection param = null,
            List<SqlStatement> additionalStatements = null,
            bool otherInitValue = false,
            bool get = true)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            UpdateStatements(statements, timestamp, param, otherInitValue, additionalStatements);
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

        private List<SqlStatement> UpdateStatements(
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var where = Rds.UsersWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateUsers(
                    where: where,
                    param: param ?? Rds.UsersParamDefault(this, otherInitValue: otherInitValue),
                    countRecord: true),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.UsersUpdated)
            });
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private SqlStatement CopyToStatement(SqlWhereCollection where, Sqls.TableTypes tableType)
        {
            var column = new Rds.UsersColumnCollection();
            var param = new Rds.UsersParamCollection();
            column.TenantId(function: Sqls.Functions.SingleColumn); param.TenantId();
            column.UserId(function: Sqls.Functions.SingleColumn); param.UserId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.LoginId(function: Sqls.Functions.SingleColumn); param.LoginId();
            column.Language(function: Sqls.Functions.SingleColumn); param.Language();
            column.DeptId(function: Sqls.Functions.SingleColumn); param.DeptId();
            column.FirstAndLastNameOrder(function: Sqls.Functions.SingleColumn); param.FirstAndLastNameOrder();
            column.TenantManager(function: Sqls.Functions.SingleColumn); param.TenantManager();
            column.ServiceManager(function: Sqls.Functions.SingleColumn); param.ServiceManager();
            column.Disabled(function: Sqls.Functions.SingleColumn); param.Disabled();
            column.Developer(function: Sqls.Functions.SingleColumn); param.Developer();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            if (!GlobalId.InitialValue())
            {
                column.GlobalId(function: Sqls.Functions.SingleColumn);
                param.GlobalId();
            }
            if (!Name.InitialValue())
            {
                column.Name(function: Sqls.Functions.SingleColumn);
                param.Name();
            }
            if (!UserCode.InitialValue())
            {
                column.UserCode(function: Sqls.Functions.SingleColumn);
                param.UserCode();
            }
            if (!Password.InitialValue())
            {
                column.Password(function: Sqls.Functions.SingleColumn);
                param.Password();
            }
            if (!LastName.InitialValue())
            {
                column.LastName(function: Sqls.Functions.SingleColumn);
                param.LastName();
            }
            if (!FirstName.InitialValue())
            {
                column.FirstName(function: Sqls.Functions.SingleColumn);
                param.FirstName();
            }
            if (!Birthday.InitialValue())
            {
                column.Birthday(function: Sqls.Functions.SingleColumn);
                param.Birthday();
            }
            if (!Gender.InitialValue())
            {
                column.Gender(function: Sqls.Functions.SingleColumn);
                param.Gender();
            }
            if (!TimeZone.InitialValue())
            {
                column.TimeZone(function: Sqls.Functions.SingleColumn);
                param.TimeZone();
            }
            if (!Body.InitialValue())
            {
                column.Body(function: Sqls.Functions.SingleColumn);
                param.Body();
            }
            if (!LastLoginTime.InitialValue())
            {
                column.LastLoginTime(function: Sqls.Functions.SingleColumn);
                param.LastLoginTime();
            }
            if (!PasswordExpirationTime.InitialValue())
            {
                column.PasswordExpirationTime(function: Sqls.Functions.SingleColumn);
                param.PasswordExpirationTime();
            }
            if (!PasswordChangeTime.InitialValue())
            {
                column.PasswordChangeTime(function: Sqls.Functions.SingleColumn);
                param.PasswordChangeTime();
            }
            if (!NumberOfLogins.InitialValue())
            {
                column.NumberOfLogins(function: Sqls.Functions.SingleColumn);
                param.NumberOfLogins();
            }
            if (!NumberOfDenial.InitialValue())
            {
                column.NumberOfDenial(function: Sqls.Functions.SingleColumn);
                param.NumberOfDenial();
            }
            if (!UserSettings.InitialValue())
            {
                column.UserSettings(function: Sqls.Functions.SingleColumn);
                param.UserSettings();
            }
            if (!ApiKey.InitialValue())
            {
                column.ApiKey(function: Sqls.Functions.SingleColumn);
                param.ApiKey();
            }
            if (!Comments.InitialValue())
            {
                column.Comments(function: Sqls.Functions.SingleColumn);
                param.Comments();
            }
            return Rds.InsertUsers(
                tableType: tableType,
                param: param,
                select: Rds.SelectUsers(column: column, where: where),
                addUpdatorParam: false);
        }

        public Error.Types Delete(SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.UsersWhere().UserId(UserId);
            statements.AddRange(new List<SqlStatement>
            {
                CopyToStatement(where, Sqls.TableTypes.Deleted),
                Rds.PhysicalDeleteUsers(where: where),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.UsersUpdated)
            });
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
            var userHash = SiteInfo.TenantCaches[Sessions.TenantId()].UserHash;
            if (userHash.Keys.Contains(UserId))
            {
                userHash.Remove(UserId);
            }
            return Error.Types.None;
        }

        public Error.Types Restore(SiteSettings ss,int userId)
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
            SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
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
                    case "Users_ApiKey": ApiKey = Forms.Data(controlId).ToString(); break;
                    case "Users_OldPassword": OldPassword = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_ChangedPassword": ChangedPassword = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_ChangedPasswordValidator": ChangedPasswordValidator = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_AfterResetPassword": AfterResetPassword = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_AfterResetPasswordValidator": AfterResetPasswordValidator = Forms.Data(controlId).ToString().Sha512Cng(); break;
                    case "Users_DemoMailAddress": DemoMailAddress = Forms.Data(controlId).ToString(); break;
                    case "Users_SessionGuid": SessionGuid = Forms.Data(controlId).ToString(); break;
                    case "Users_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments.Prepend(Forms.Data("Comments")); break;
                    case "VerUp": VerUp = Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                controlId.Substring("Comment".Length).ToInt(),
                                Forms.Data(controlId));
                        }
                        break;
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

        public void SetByApi(SiteSettings ss)
        {
            var data = Forms.String().Deserialize<UserApiModel>();
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
            if (data.Birthday != null) Birthday = new Time(data.Birthday.ToDateTime(), byForm: true);
            if (data.Gender != null) Gender = data.Gender.ToString().ToString();
            if (data.Language != null) Language = data.Language.ToString().ToString();
            if (data.TimeZone != null) TimeZone = data.TimeZone.ToString().ToString();
            if (data.DeptId != null) DeptId = data.DeptId.ToInt().ToInt();
            if (data.FirstAndLastNameOrder != null) FirstAndLastNameOrder = (Names.FirstAndLastNameOrders)data.FirstAndLastNameOrder.ToInt().ToInt();
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.LastLoginTime != null) LastLoginTime = new Time(data.LastLoginTime.ToDateTime(), byForm: true);
            if (data.PasswordExpirationTime != null) PasswordExpirationTime = new Time(data.PasswordExpirationTime.ToDateTime(), byForm: true);
            if (data.PasswordChangeTime != null) PasswordChangeTime = new Time(data.PasswordChangeTime.ToDateTime(), byForm: true);
            if (data.NumberOfLogins != null) NumberOfLogins = data.NumberOfLogins.ToInt().ToInt();
            if (data.NumberOfDenial != null) NumberOfDenial = data.NumberOfDenial.ToInt().ToInt();
            if (data.TenantManager != null) TenantManager = data.TenantManager.ToBool().ToBool();
            if (data.Disabled != null) Disabled = data.Disabled.ToBool().ToBool();
            if (data.ApiKey != null) ApiKey = data.ApiKey.ToString().ToString();
            if (data.Comments != null) Comments.Prepend(data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
        }

        private void SetBySession()
        {
            if (!Forms.HasData("Users_UserSettings")) UserSettings = Session_UserSettings();
            if (!Forms.HasData("Users_MailAddresses")) MailAddresses = Session_MailAddresses();
        }

        private void Set(SiteSettings ss,DataTable dataTable)
        {
            switch (dataTable.Rows.Count)
            {
                case 1: Set(ss, dataTable.Rows[0]); break;
                case 0: AccessStatus = Databases.AccessStatuses.NotFound; break;
                default: AccessStatus = Databases.AccessStatuses.Overlap; break;
            }
        }

        private void Set(SiteSettings ss,DataRow dataRow, string tableAlias = null)
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
                            Birthday = new Time(dataRow, column.ColumnName);
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
                            LastLoginTime = new Time(dataRow, column.ColumnName);
                            SavedLastLoginTime = LastLoginTime.Value;
                            break;
                        case "PasswordExpirationTime":
                            PasswordExpirationTime = new Time(dataRow, column.ColumnName);
                            SavedPasswordExpirationTime = PasswordExpirationTime.Value;
                            break;
                        case "PasswordChangeTime":
                            PasswordChangeTime = new Time(dataRow, column.ColumnName);
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
                        case "Comments":
                            Comments = dataRow[column.ColumnName].ToString().Deserialize<Comments>() ?? new Comments();
                            SavedComments = Comments.ToJson();
                            break;
                        case "Creator":
                            Creator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedCreator = Creator.Id;
                            break;
                        case "Updator":
                            Updator = SiteInfo.User(dataRow[column.ColumnName].ToInt());
                            SavedUpdator = Updator.Id;
                            break;
                        case "CreatedTime":
                            CreatedTime = new Time(dataRow, column.ColumnName);
                            SavedCreatedTime = CreatedTime.Value;
                            break;
                        case "UpdatedTime":
                            UpdatedTime = new Time(dataRow, column.ColumnName); Timestamp = dataRow.Field<DateTime>(column.ColumnName).ToString("yyyy/M/d H:m:s.fff");
                            SavedUpdatedTime = UpdatedTime.Value;
                            break;
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated()
        {
            return
                TenantId_Updated() ||
                UserId_Updated() ||
                Ver_Updated() ||
                LoginId_Updated() ||
                GlobalId_Updated() ||
                Name_Updated() ||
                UserCode_Updated() ||
                Password_Updated() ||
                LastName_Updated() ||
                FirstName_Updated() ||
                Birthday_Updated() ||
                Gender_Updated() ||
                Language_Updated() ||
                TimeZone_Updated() ||
                DeptId_Updated() ||
                FirstAndLastNameOrder_Updated() ||
                Body_Updated() ||
                LastLoginTime_Updated() ||
                PasswordExpirationTime_Updated() ||
                PasswordChangeTime_Updated() ||
                NumberOfLogins_Updated() ||
                NumberOfDenial_Updated() ||
                TenantManager_Updated() ||
                ServiceManager_Updated() ||
                Disabled_Updated() ||
                Developer_Updated() ||
                UserSettings_Updated() ||
                ApiKey_Updated() ||
                Comments_Updated() ||
                Creator_Updated() ||
                Updator_Updated();
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
            var statements = new List<SqlStatement>();
            Session_MailAddresses()?.ForEach(mailAddress =>
                statements.Add(Rds.InsertMailAddresses(
                    param: Rds.MailAddressesParam()
                        .OwnerId(UserId)
                        .OwnerType("Users")
                        .MailAddress(mailAddress))));
            if (statements.Any())
            {
                statements.Insert(0, Rds.PhysicalDeleteMailAddresses(
                    where: Rds.MailAddressesWhere()
                        .OwnerId(UserId)
                        .OwnerType("Users")));
                Rds.ExecuteNonQuery(transactional: true, statements: statements.ToArray());
            }
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
            Sessions.Set("UserId", UserId);
            Sessions.Set("Language", Language);
            Sessions.Set("Developer", Developer);
            Sessions.Set("TimeZoneInfo", TimeZoneInfo);
            Sessions.Set("RdsUser", RdsUser());
            Sessions.Set("UserSettings", UserSettings.ToJson());
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
        public string ToExport(Column column, ExportColumn exportColumn = null)
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types CreateApiKey(SiteSettings ss)
        {
            ApiKey = Guid.NewGuid().ToString().Sha512Cng();
            return Update(ss);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public Error.Types DeleteApiKey(SiteSettings ss)
        {
            ApiKey = string.Empty;
            return Update(ss);
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool InitialValue()
        {
            return UserId == 0;
        }
    }
}
