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
    public class RegistrationModel : BaseModel
    {
        public int TenantId = 0;
        public int RegistrationId = 0;
        public string MailAddress = string.Empty;
        public int Invitee = 0;
        public string InviteeName = string.Empty;
        public string LoginId = string.Empty;
        public string Name = string.Empty;
        public string Password = string.Empty;
        public string PasswordValidate = string.Empty;
        public string Language = "ja";
        public string Passphrase = string.Empty;
        public string Invitingflg = string.Empty;
        public int UserId = 0;
        public int DeptId = 0;
        public int GroupId = 0;
        public int SavedTenantId = 0;
        public int SavedRegistrationId = 0;
        public string SavedMailAddress = string.Empty;
        public int SavedInvitee = 0;
        public string SavedInviteeName = string.Empty;
        public string SavedLoginId = string.Empty;
        public string SavedName = string.Empty;
        public string SavedPassword = string.Empty;
        public string SavedPasswordValidate = string.Empty;
        public string SavedLanguage = "ja";
        public string SavedPassphrase = string.Empty;
        public string SavedInvitingflg = string.Empty;
        public int SavedUserId = 0;
        public int SavedDeptId = 0;
        public int SavedGroupId = 0;

        public bool TenantId_Updated(Context context, Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool RegistrationId_Updated(Context context, Column column = null)
        {
            return RegistrationId != SavedRegistrationId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != RegistrationId);
        }

        public bool MailAddress_Updated(Context context, Column column = null)
        {
            return MailAddress != SavedMailAddress && MailAddress != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != MailAddress);
        }

        public bool Invitee_Updated(Context context, Column column = null)
        {
            return Invitee != SavedInvitee &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != Invitee);
        }

        public bool InviteeName_Updated(Context context, Column column = null)
        {
            return InviteeName != SavedInviteeName && InviteeName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != InviteeName);
        }

        public bool LoginId_Updated(Context context, Column column = null)
        {
            return LoginId != SavedLoginId && LoginId != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != LoginId);
        }

        public bool Name_Updated(Context context, Column column = null)
        {
            return Name != SavedName && Name != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Name);
        }

        public bool Password_Updated(Context context, Column column = null)
        {
            return Password != SavedPassword && Password != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Password);
        }

        public bool Language_Updated(Context context, Column column = null)
        {
            return Language != SavedLanguage && Language != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Language);
        }

        public bool Passphrase_Updated(Context context, Column column = null)
        {
            return Passphrase != SavedPassphrase && Passphrase != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Passphrase);
        }

        public bool Invitingflg_Updated(Context context, Column column = null)
        {
            return Invitingflg != SavedInvitingflg && Invitingflg != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Invitingflg);
        }

        public bool UserId_Updated(Context context, Column column = null)
        {
            return UserId != SavedUserId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != UserId);
        }

        public bool DeptId_Updated(Context context, Column column = null)
        {
            return DeptId != SavedDeptId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != DeptId);
        }

        public bool GroupId_Updated(Context context, Column column = null)
        {
            return GroupId != SavedGroupId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != GroupId);
        }

        public List<int> SwitchTargets;

        public RegistrationModel()
        {
        }

        public RegistrationModel(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData = null,
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

        public RegistrationModel(
            Context context,
            SiteSettings ss,
            int registrationId,
            Dictionary<string, string> formData = null,
            bool setByApi = false,
            bool clearSessions = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            RegistrationId = registrationId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    where: Rds.RegistrationsWhereDefault(this)
                        .Registrations_Ver(context.QueryStrings.Int("ver")), ss: ss);
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

        public RegistrationModel(
            Context context,
            SiteSettings ss,
            DataRow dataRow,
            Dictionary<string, string> formData = null,
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

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
        }

        public RegistrationModel Get(
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
                statements: Rds.SelectRegistrations(
                    tableType: tableType,
                    column: column ?? Rds.RegistrationsDefaultColumns(),
                    join: join ??  Rds.RegistrationsJoinDefault(),
                    where: where ?? Rds.RegistrationsWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public RegistrationApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new RegistrationApiModel()
            {
                ApiVersion = context.ApiVersion
            };
            ss.ReadableColumns(noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "TenantId": data.TenantId = TenantId; break;
                    case "RegistrationId": data.RegistrationId = RegistrationId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "MailAddress": data.MailAddress = MailAddress; break;
                    case "Invitee": data.Invitee = Invitee; break;
                    case "InviteeName": data.InviteeName = InviteeName; break;
                    case "LoginId": data.LoginId = LoginId; break;
                    case "Name": data.Name = Name; break;
                    case "Password": data.Password = Password; break;
                    case "Language": data.Language = Language; break;
                    case "Passphrase": data.Passphrase = Passphrase; break;
                    case "Invitingflg": data.Invitingflg = Invitingflg; break;
                    case "UserId": data.UserId = UserId; break;
                    case "DeptId": data.DeptId = DeptId; break;
                    case "GroupId": data.GroupId = GroupId; break;
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
            var statements = new List<SqlStatement>();
            statements.AddRange(CreateStatements(
                context: context,
                ss: ss,
                tableType: tableType,
                param: param,
                otherInitValue: otherInitValue));
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            RegistrationId = (response.Id ?? RegistrationId).ToInt();
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
                Rds.InsertRegistrations(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.RegistrationsParamDefault(
                        context: context,
                        registrationModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
            });
            statements.AddRange(UpdateAttachmentsStatements(context: context));
            return statements;
        }

        public ErrorData Update(
            Context context,
            SiteSettings ss,
            IEnumerable<string> permissions = null,
            bool permissionChanged = false,
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
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Event == "Conflicted")
            {
                return new ErrorData(
                    type: Error.Types.UpdateConflicts,
                    id: RegistrationId);
            }
            if (get)
            {
                Get(context: context, ss: ss);
            }
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
            var where = Rds.RegistrationsWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (Versions.VerUp(
                context: context,
                ss: ss,
                verUp: VerUp))
            {
                statements.Add(Rds.RegistrationsCopyToStatement(
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
                Rds.UpdateRegistrations(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.RegistrationsParamDefault(
                        context: context,
                        registrationModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(RegistrationId)) {
                    IfConflicted = true,
                    Id = RegistrationId
                },
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
                        referenceId: RegistrationId));
            return statements;
        }

        public ErrorData UpdateOrCreate(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertRegistrations(
                    where: where ?? Rds.RegistrationsWhereDefault(this),
                    param: param ?? Rds.RegistrationsParamDefault(
                        context: context, registrationModel: this, setDefault: true)),
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            RegistrationId = (response.Id ?? RegistrationId).ToInt();
            Get(context: context, ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.RegistrationsWhere().RegistrationId(RegistrationId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteRegistrations(
                    factory: context,
                    where: where),
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, SiteSettings ss,int registrationId)
        {
            RegistrationId = registrationId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreRegistrations(
                        factory: context,
                        where: Rds.RegistrationsWhere().RegistrationId(RegistrationId)),
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteRegistrations(
                    tableType: tableType,
                    param: Rds.RegistrationsParam().RegistrationId(RegistrationId)));
            return new ErrorData(type: Error.Types.None);
        }

        public void SetByForm(
            Context context,
            SiteSettings ss,
            Dictionary<string, string> formData)
        {
            formData.ForEach(data =>
            {
                var key = data.Key;
                var value = data.Value ?? string.Empty;
                switch (key)
                {
                    case "Registrations_MailAddress": MailAddress = value.ToString(); break;
                    case "Registrations_Invitee": Invitee = value.ToInt(); break;
                    case "Registrations_InviteeName": InviteeName = value.ToString(); break;
                    case "Registrations_LoginId": LoginId = value.ToString(); break;
                    case "Registrations_Name": Name = value.ToString(); break;
                    case "Registrations_Password": Password = value.ToString().Sha512Cng(); break;
                    case "Registrations_PasswordValidate": PasswordValidate = value.ToString().Sha512Cng(); break;
                    case "Registrations_Language": Language = value.ToString(); break;
                    case "Registrations_Passphrase": Passphrase = value.ToString(); break;
                    case "Registrations_Invitingflg": Invitingflg = value.ToString(); break;
                    case "Registrations_UserId": UserId = value.ToInt(); break;
                    case "Registrations_DeptId": DeptId = value.ToInt(); break;
                    case "Registrations_GroupId": GroupId = value.ToInt(); break;
                    case "Registrations_Timestamp": Timestamp = value.ToString(); break;
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

        public void SetByModel(RegistrationModel registrationModel)
        {
            TenantId = registrationModel.TenantId;
            MailAddress = registrationModel.MailAddress;
            Invitee = registrationModel.Invitee;
            InviteeName = registrationModel.InviteeName;
            LoginId = registrationModel.LoginId;
            Name = registrationModel.Name;
            Password = registrationModel.Password;
            PasswordValidate = registrationModel.PasswordValidate;
            Language = registrationModel.Language;
            Passphrase = registrationModel.Passphrase;
            Invitingflg = registrationModel.Invitingflg;
            UserId = registrationModel.UserId;
            DeptId = registrationModel.DeptId;
            GroupId = registrationModel.GroupId;
            Comments = registrationModel.Comments;
            Creator = registrationModel.Creator;
            Updator = registrationModel.Updator;
            CreatedTime = registrationModel.CreatedTime;
            UpdatedTime = registrationModel.UpdatedTime;
            VerUp = registrationModel.VerUp;
            Comments = registrationModel.Comments;
            ClassHash = registrationModel.ClassHash;
            NumHash = registrationModel.NumHash;
            DateHash = registrationModel.DateHash;
            DescriptionHash = registrationModel.DescriptionHash;
            CheckHash = registrationModel.CheckHash;
            AttachmentsHash = registrationModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss)
        {
            var data = context.RequestDataString.Deserialize<RegistrationApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.MailAddress != null) MailAddress = data.MailAddress.ToString().ToString();
            if (data.Invitee != null) Invitee = data.Invitee.ToInt().ToInt();
            if (data.InviteeName != null) InviteeName = data.InviteeName.ToString().ToString();
            if (data.LoginId != null) LoginId = data.LoginId.ToString().ToString();
            if (data.Name != null) Name = data.Name.ToString().ToString();
            if (data.Password != null) Password = data.Password.ToString().ToString().Sha512Cng();
            if (data.Language != null) Language = data.Language.ToString().ToString();
            if (data.Passphrase != null) Passphrase = data.Passphrase.ToString().ToString();
            if (data.Invitingflg != null) Invitingflg = data.Invitingflg.ToString().ToString();
            if (data.UserId != null) UserId = data.UserId.ToInt().ToInt();
            if (data.DeptId != null) DeptId = data.DeptId.ToInt().ToInt();
            if (data.GroupId != null) GroupId = data.GroupId.ToInt().ToInt();
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
                value: o.Value.ToDateTime().ToUniversal(context: context)));
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
                        case "RegistrationId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                RegistrationId = dataRow[column.ColumnName].ToInt();
                                SavedRegistrationId = RegistrationId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "MailAddress":
                            MailAddress = dataRow[column.ColumnName].ToString();
                            SavedMailAddress = MailAddress;
                            break;
                        case "Invitee":
                            Invitee = dataRow[column.ColumnName].ToInt();
                            SavedInvitee = Invitee;
                            break;
                        case "InviteeName":
                            InviteeName = dataRow[column.ColumnName].ToString();
                            SavedInviteeName = InviteeName;
                            break;
                        case "LoginId":
                            LoginId = dataRow[column.ColumnName].ToString();
                            SavedLoginId = LoginId;
                            break;
                        case "Name":
                            Name = dataRow[column.ColumnName].ToString();
                            SavedName = Name;
                            break;
                        case "Password":
                            Password = dataRow[column.ColumnName].ToString();
                            SavedPassword = Password;
                            break;
                        case "Language":
                            Language = dataRow[column.ColumnName].ToString();
                            SavedLanguage = Language;
                            break;
                        case "Passphrase":
                            Passphrase = dataRow[column.ColumnName].ToString();
                            SavedPassphrase = Passphrase;
                            break;
                        case "Invitingflg":
                            Invitingflg = dataRow[column.ColumnName].ToString();
                            SavedInvitingflg = Invitingflg;
                            break;
                        case "UserId":
                            UserId = dataRow[column.ColumnName].ToInt();
                            SavedUserId = UserId;
                            break;
                        case "DeptId":
                            DeptId = dataRow[column.ColumnName].ToInt();
                            SavedDeptId = DeptId;
                            break;
                        case "GroupId":
                            GroupId = dataRow[column.ColumnName].ToInt();
                            SavedGroupId = GroupId;
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
                || RegistrationId_Updated(context: context)
                || Ver_Updated(context: context)
                || MailAddress_Updated(context: context)
                || Invitee_Updated(context: context)
                || InviteeName_Updated(context: context)
                || LoginId_Updated(context: context)
                || Name_Updated(context: context)
                || Password_Updated(context: context)
                || Language_Updated(context: context)
                || Passphrase_Updated(context: context)
                || Invitingflg_Updated(context: context)
                || UserId_Updated(context: context)
                || DeptId_Updated(context: context)
                || GroupId_Updated(context: context)
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
        public Title Title
        {
            get
            {
                return new Title(MailAddress);
            }
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public void Approval(Context context)
        {
            Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertUsers(
                        selectIdentity: true,
                        param: Rds.UsersParam()
                            .TenantId(TenantId)
                            .LoginId(LoginId)
                            .Password(Password)
                            .Name(Name)
                            .Language(Language)
                            ),
                    Rds.InsertMailAddresses(
                        param: Rds.MailAddressesParam()
                            .OwnerId(raw: Def.Sql.Identity)
                            .OwnerType("Users")
                            .MailAddress(MailAddress))
                }).Id.ToInt();
            SiteInfo.Reflesh(
                context: context,
                force: true);
        }
    }
}
