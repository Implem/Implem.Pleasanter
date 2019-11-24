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
    public class GroupModel : BaseModel
    {
        public int TenantId = 0;
        public int GroupId = 0;
        public string GroupName = string.Empty;
        public string Body = string.Empty;

        public Title Title
        {
            get
            {
                return new Title(GroupId, GroupName);
            }
        }

        public int SavedTenantId = 0;
        public int SavedGroupId = 0;
        public string SavedGroupName = string.Empty;
        public string SavedBody = string.Empty;

        public bool TenantId_Updated(Context context, Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool GroupId_Updated(Context context, Column column = null)
        {
            return GroupId != SavedGroupId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != GroupId);
        }

        public bool GroupName_Updated(Context context, Column column = null)
        {
            return GroupName != SavedGroupName && GroupName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != GroupName);
        }

        public bool Body_Updated(Context context, Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Body);
        }

        public List<int> SwitchTargets;

        public GroupModel()
        {
        }

        public GroupModel(
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

        public GroupModel(
            Context context,
            SiteSettings ss,
            int groupId,
            IDictionary<string, string> formData = null,
            bool setByApi = false,
            bool clearSessions = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            GroupId = groupId;
            if (context.QueryStrings.ContainsKey("ver"))
            {
                Get(context: context,
                    tableType: Sqls.TableTypes.NormalAndHistory,
                    where: Rds.GroupsWhereDefault(this)
                        .Groups_Ver(context.QueryStrings.Int("ver")), ss: ss);
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

        public GroupModel(
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

        private void OnConstructing(Context context)
        {
        }

        private void OnConstructed(Context context)
        {
        }

        public void ClearSessions(Context context)
        {
        }

        public GroupModel Get(
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
                statements: Rds.SelectGroups(
                    tableType: tableType,
                    column: column ?? Rds.GroupsDefaultColumns(),
                    join: join ??  Rds.GroupsJoinDefault(),
                    where: where ?? Rds.GroupsWhereDefault(this),
                    orderBy: orderBy,
                    param: param,
                    distinct: distinct,
                    top: top)));
            return this;
        }

        public GroupApiModel GetByApi(Context context, SiteSettings ss)
        {
            var data = new GroupApiModel()
            {
                ApiVersion = context.ApiVersion
            };
            ss.ReadableColumns(noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "TenantId": data.TenantId = TenantId; break;
                    case "GroupId": data.GroupId = GroupId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "GroupName": data.GroupName = GroupName; break;
                    case "Body": data.Body = Body; break;
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
            GroupId = (response.Id ?? GroupId).ToInt();
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
                Rds.InsertGroups(
                    dataTableName: dataTableName,
                    tableType: tableType,
                    selectIdentity: true,
                    param: param ?? Rds.GroupsParamDefault(
                        context: context,
                        groupModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
                    Rds.InsertGroupMembers(
                        tableType: tableType,
                        param: param ?? Rds.GroupMembersParam()
                            .GroupId(raw: Def.Sql.Identity)
                            .UserId(context.UserId)
                            .Admin(true)),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.GroupsUpdated),
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
                    id: GroupId);
            }
            if (get)
            {
                Get(context: context, ss: ss);
            }
            statements = new List<SqlStatement>
            {
                Rds.PhysicalDeleteGroupMembers(
                    where: Rds.GroupMembersWhere()
                        .GroupId(GroupId))
            };
            context.Forms.List("CurrentMembersAll").ForEach(data =>
            {
                if (data.StartsWith("Dept,"))
                {
                    statements.Add(Rds.InsertGroupMembers(
                        param: Rds.GroupMembersParam()
                            .GroupId(GroupId)
                            .DeptId(data.Split_2nd().ToInt())
                            .Admin(data.Split_3rd().ToBool())));
                }
                if (data.StartsWith("User,"))
                {
                    statements.Add(Rds.InsertGroupMembers(
                        param: Rds.GroupMembersParam()
                            .GroupId(GroupId)
                            .UserId(data.Split_2nd().ToInt())
                            .Admin(data.Split_3rd().ToBool())));
                }
            });
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: statements.ToArray());
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
            var where = Rds.GroupsWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(Rds.GroupsCopyToStatement(
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
                Rds.UpdateGroups(
                    dataTableName: dataTableName,
                    where: where,
                    param: param ?? Rds.GroupsParamDefault(
                        context: context,
                        groupModel: this,
                        otherInitValue: otherInitValue)),
                new SqlStatement(Def.Sql.IfConflicted.Params(GroupId))
                {
                    IfConflicted = true
                },
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.GroupsUpdated),
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
                        referenceId: GroupId));
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
                Rds.UpdateOrInsertGroups(
                    where: where ?? Rds.GroupsWhereDefault(this),
                    param: param ?? Rds.GroupsParamDefault(
                        context: context, groupModel: this, setDefault: true)),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.GroupsUpdated),
            };
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            GroupId = (response.Id ?? GroupId).ToInt();
            Get(context: context, ss: ss);
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.GroupsWhere().GroupId(GroupId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteGroups(factory: context, where: where),
                Rds.PhysicalDeleteGroupMembers(
                    where: Rds.GroupMembersWhere()
                        .GroupId(GroupId)),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.GroupsUpdated),
            });
            var response = Repository.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData Restore(Context context, SiteSettings ss,int groupId)
        {
            GroupId = groupId;
            Repository.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreGroups(
                        factory: context,
                        where: Rds.GroupsWhere().GroupId(GroupId)),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.GroupsUpdated),
                });
            return new ErrorData(type: Error.Types.None);
        }

        public ErrorData PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Repository.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteGroups(
                    tableType: tableType,
                    param: Rds.GroupsParam().GroupId(GroupId)));
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
                    case "Groups_TenantId": TenantId = value.ToInt(); break;
                    case "Groups_GroupName": GroupName = value.ToString(); break;
                    case "Groups_Body": Body = value.ToString(); break;
                    case "Groups_Timestamp": Timestamp = value.ToString(); break;
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

        public void SetByModel(GroupModel groupModel)
        {
            TenantId = groupModel.TenantId;
            GroupName = groupModel.GroupName;
            Body = groupModel.Body;
            Comments = groupModel.Comments;
            Creator = groupModel.Creator;
            Updator = groupModel.Updator;
            CreatedTime = groupModel.CreatedTime;
            UpdatedTime = groupModel.UpdatedTime;
            VerUp = groupModel.VerUp;
            Comments = groupModel.Comments;
            ClassHash = groupModel.ClassHash;
            NumHash = groupModel.NumHash;
            DateHash = groupModel.DateHash;
            DescriptionHash = groupModel.DescriptionHash;
            CheckHash = groupModel.CheckHash;
            AttachmentsHash = groupModel.AttachmentsHash;
        }

        public void SetByApi(Context context, SiteSettings ss)
        {
            var data = context.RequestDataString.Deserialize<GroupApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.TenantId != null) TenantId = data.TenantId.ToInt().ToInt();
            if (data.GroupName != null) GroupName = data.GroupName.ToString().ToString();
            if (data.Body != null) Body = data.Body.ToString().ToString();
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
            data.AttachmentsHash?.ForEach(o => Attachments(
                columnName: o.Key,
                value: o.Value));
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
                        case "GroupId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                GroupId = dataRow[column.ColumnName].ToInt();
                                SavedGroupId = GroupId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "GroupName":
                            GroupName = dataRow[column.ColumnName].ToString();
                            SavedGroupName = GroupName;
                            break;
                        case "Body":
                            Body = dataRow[column.ColumnName].ToString();
                            SavedBody = Body;
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
                || GroupId_Updated(context: context)
                || Ver_Updated(context: context)
                || GroupName_Updated(context: context)
                || Body_Updated(context: context)
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
        public System.Web.Mvc.ContentResult GetByApi(Context context)
        {
            return GroupUtilities.GetByApi(
                context: context,
                ss: SiteSettingsUtilities.ApiGroupsSiteSettings(context));
        }
    }
}
