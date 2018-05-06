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
        public int TenantId = Sessions.TenantId();
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

        [NonSerialized] public int SavedTenantId = Sessions.TenantId();
        [NonSerialized] public int SavedGroupId = 0;
        [NonSerialized] public string SavedGroupName = string.Empty;
        [NonSerialized] public string SavedBody = string.Empty;

        public bool TenantId_Updated(Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != TenantId);
        }

        public bool GroupId_Updated(Column column = null)
        {
            return GroupId != SavedGroupId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != GroupId);
        }

        public bool GroupName_Updated(Column column = null)
        {
            return GroupName != SavedGroupName && GroupName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != GroupName);
        }

        public bool Body_Updated(Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Body);
        }

        public List<int> SwitchTargets;

        public GroupModel()
        {
        }

        public GroupModel(
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

        public GroupModel(
            SiteSettings ss,
            int groupId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            GroupId = groupId;
            Get(ss);
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm(ss);
            if (setByApi) SetByApi(ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public GroupModel(SiteSettings ss, DataRow dataRow, string tableAlias = null)
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
        }

        public GroupModel Get(
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
            Set(ss, Rds.ExecuteTable(statements: Rds.SelectGroups(
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

        public GroupApiModel GetByApi(SiteSettings ss)
        {
            var data = new GroupApiModel();
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
            var statements = new List<SqlStatement>();
            CreateStatements(statements, ss, tableType, param, otherInitValue);
            var newId = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            GroupId = newId != 0 ? newId : GroupId;
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
                Rds.InsertGroups(
                    tableType: tableType,
                        selectIdentity: true,
                    param: param ?? Rds.GroupsParamDefault(
                        this, setDefault: true, otherInitValue: otherInitValue)),
                    Rds.InsertGroupMembers(
                        tableType: tableType,
                        param: param ?? Rds.GroupMembersParam()
                            .GroupId(raw: Def.Sql.Identity)
                            .UserId(Sessions.UserId())
                            .Admin(true)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.GroupsUpdated)
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
            var count = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            if (count == 0) return Error.Types.UpdateConflicts;
            if (get) Get(ss);
            statements = new List<SqlStatement>
            {
                Rds.PhysicalDeleteGroupMembers(
                    where: Rds.GroupMembersWhere()
                        .GroupId(GroupId))
            };
            Forms.List("CurrentMembersAll").ForEach(data =>
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
            Rds.ExecuteNonQuery(transactional: true, statements: statements.ToArray());
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var where = Rds.GroupsWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateGroups(
                    where: where,
                    param: param ?? Rds.GroupsParamDefault(this, otherInitValue: otherInitValue),
                    countRecord: true),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.GroupsUpdated)
            });
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private SqlStatement CopyToStatement(SqlWhereCollection where, Sqls.TableTypes tableType)
        {
            var column = new Rds.GroupsColumnCollection();
            var param = new Rds.GroupsParamCollection();
            column.TenantId(function: Sqls.Functions.SingleColumn); param.TenantId();
            column.GroupId(function: Sqls.Functions.SingleColumn); param.GroupId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.GroupName(function: Sqls.Functions.SingleColumn); param.GroupName();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            if (!Body.InitialValue())
            {
                column.Body(function: Sqls.Functions.SingleColumn);
                param.Body();
            }
            if (!Comments.InitialValue())
            {
                column.Comments(function: Sqls.Functions.SingleColumn);
                param.Comments();
            }
            return Rds.InsertGroups(
                tableType: tableType,
                param: param,
                select: Rds.SelectGroups(column: column, where: where),
                addUpdatorParam: false);
        }

        public Error.Types UpdateOrCreate(
            SiteSettings ss,
            RdsUser rdsUser = null,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertGroups(
                    selectIdentity: true,
                    where: where ?? Rds.GroupsWhereDefault(this),
                    param: param ?? Rds.GroupsParamDefault(this, setDefault: true)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.GroupsUpdated)
            };
            var newId = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            GroupId = newId != 0 ? newId : GroupId;
            Get(ss);
            return Error.Types.None;
        }

        public Error.Types Delete(SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.GroupsWhere().GroupId(GroupId);
            statements.AddRange(new List<SqlStatement>
            {
                CopyToStatement(where, Sqls.TableTypes.Deleted),
                Rds.PhysicalDeleteGroups(where: where),
                Rds.PhysicalDeleteGroupMembers(
                    where: Rds.GroupMembersWhere()
                        .GroupId(GroupId)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.GroupsUpdated)
            });
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
            return Error.Types.None;
        }

        public Error.Types Restore(SiteSettings ss,int groupId)
        {
            GroupId = groupId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreGroups(
                        where: Rds.GroupsWhere().GroupId(GroupId)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.GroupsUpdated)
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteGroups(
                    tableType: tableType,
                    param: Rds.GroupsParam().GroupId(GroupId)));
            return Error.Types.None;
        }

        public void SetByForm(SiteSettings ss)
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Groups_TenantId": TenantId = Forms.Data(controlId).ToInt(); break;
                    case "Groups_GroupName": GroupName = Forms.Data(controlId).ToString(); break;
                    case "Groups_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Groups_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
            var data = Forms.String().Deserialize<GroupApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.TenantId != null) TenantId = data.TenantId.ToInt().ToInt();
            if (data.GroupName != null) GroupName = data.GroupName.ToString().ToString();
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.Comments != null) Comments.Prepend(data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
        }

        private void SetBySession()
        {
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

        private void Set(SiteSettings ss, DataRow dataRow, string tableAlias = null)
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
                GroupId_Updated() ||
                Ver_Updated() ||
                GroupName_Updated() ||
                Body_Updated() ||
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
    }
}
