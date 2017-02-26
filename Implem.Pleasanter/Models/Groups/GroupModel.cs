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
    public class GroupModel : BaseModel
    {
        public int TenantId = Sessions.TenantId();
        public int GroupId = 0;
        public string GroupName = string.Empty;
        public string Body = string.Empty;
        public Title Title { get { return new Title(GroupId, GroupName); } }
        public int SavedTenantId = Sessions.TenantId();
        public int SavedGroupId = 0;
        public string SavedGroupName = string.Empty;
        public string SavedBody = string.Empty;
        public bool TenantId_Updated { get { return TenantId != SavedTenantId; } }
        public bool GroupId_Updated { get { return GroupId != SavedGroupId; } }
        public bool GroupName_Updated { get { return GroupName != SavedGroupName && GroupName != null; } }
        public bool Body_Updated { get { return Body != SavedBody && Body != null; } }
        public List<int> SwitchTargets;

        public GroupModel()
        {
        }

        public GroupModel(
            SiteSettings ss,
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = ss;
            if (setByForm) SetByForm();
            MethodType = methodType;
            OnConstructed();
        }

        public GroupModel(
            SiteSettings ss,
            int groupId,
            bool clearSessions = false,
            bool setByForm = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            SiteSettings = ss;
            GroupId = groupId;
            Get();
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm();
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public GroupModel(
            SiteSettings ss,
            DataRow dataRow)
        {
            OnConstructing();
            SiteSettings = ss;
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
        }

        public GroupModel Get(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            bool distinct = false,
            int top = 0)
        {
            Set(Rds.ExecuteTable(statements: Rds.SelectGroups(
                tableType: tableType,
                column: column ?? Rds.GroupsDefaultColumns(),
                join: join ??  Rds.GroupsJoinDefault(),
                where: where ?? Rds.GroupsWhereDefault(this),
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public Error.Types Create(
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            var newId = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertGroups(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.GroupsParamDefault(
                            this, setDefault: true, paramAll: paramAll)),
                    Rds.InsertGroupMembers(
                        tableType: tableType,
                        param: param ?? Rds.GroupMembersParam()
                            .GroupId(raw: Def.Sql.Identity)
                            .UserId(Sessions.UserId())
                            .Admin(true))
                });
            GroupId = newId != 0 ? newId : GroupId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Update(bool paramAll = false)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateGroups(
                        verUp: VerUp,
                        where: Rds.GroupsWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.InRange()),
                        param: Rds.GroupsParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return Error.Types.UpdateConflicts;
            Get();
            var statements = new List<SqlStatement>
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

        public Error.Types UpdateOrCreate(
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var newId = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateOrInsertGroups(
                        selectIdentity: true,
                        where: where ?? Rds.GroupsWhereDefault(this),
                        param: param ?? Rds.GroupsParamDefault(this, setDefault: true))
                });
            GroupId = newId != 0 ? newId : GroupId;
            Get();
            return Error.Types.None;
        }

        public Error.Types Delete()
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteGroups(
                        where: Rds.GroupsWhere().GroupId(GroupId)),
                    Rds.PhysicalDeleteGroupMembers(
                        where: Rds.GroupMembersWhere()
                            .GroupId(GroupId))
                });
            return Error.Types.None;
        }

        public Error.Types Restore(int groupId)
        {
            GroupId = groupId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreGroups(
                        where: Rds.GroupsWhere().GroupId(GroupId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteGroups(
                    tableType: tableType,
                    param: Rds.GroupsParam().GroupId(GroupId)));
            return Error.Types.None;
        }

        private void SetByForm()
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Groups_TenantId": TenantId = Forms.Data(controlId).ToInt(); break;
                    case "Groups_GroupName": GroupName = Forms.Data(controlId).ToString(); break;
                    case "Groups_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Groups_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
                    case "GroupId": if (dataRow[name] != DBNull.Value) { GroupId = dataRow[name].ToInt(); SavedGroupId = GroupId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "GroupName": GroupName = dataRow[name].ToString(); SavedGroupName = GroupName; break;
                    case "Body": Body = dataRow[name].ToString(); SavedBody = Body; break;
                    case "Comments": Comments = dataRow["Comments"].ToString().Deserialize<Comments>() ?? new Comments(); SavedComments = Comments.ToJson(); break;
                    case "Creator": Creator = SiteInfo.User(dataRow.Int(name)); SavedCreator = Creator.Id; break;
                    case "Updator": Updator = SiteInfo.User(dataRow.Int(name)); SavedUpdator = Updator.Id; break;
                    case "CreatedTime": CreatedTime = new Time(dataRow, "CreatedTime"); SavedCreatedTime = CreatedTime.Value; break;
                    case "UpdatedTime": UpdatedTime = new Time(dataRow, "UpdatedTime"); Timestamp = dataRow.Field<DateTime>("UpdatedTime").ToString("yyyy/M/d H:m:s.fff"); SavedUpdatedTime = UpdatedTime.Value; break;
                    case "IsHistory": VerType = dataRow[name].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                }
            }
        }
    }
}
