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
    public class DeptModel : BaseModel, Interfaces.IConvertable
    {
        public int TenantId = Sessions.TenantId();
        public int DeptId = 0;
        public string DeptCode = string.Empty;
        public string DeptName = string.Empty;
        public string Body = string.Empty;

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
                return new Title(DeptId, DeptName);
            }
        }

        [NonSerialized] public int SavedTenantId = Sessions.TenantId();
        [NonSerialized] public int SavedDeptId = 0;
        [NonSerialized] public string SavedDeptCode = string.Empty;
        [NonSerialized] public string SavedDeptName = string.Empty;
        [NonSerialized] public string SavedBody = string.Empty;

        public bool TenantId_Updated(Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != TenantId);
        }

        public bool DeptId_Updated(Column column = null)
        {
            return DeptId != SavedDeptId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToInt() != DeptId);
        }

        public bool DeptCode_Updated(Column column = null)
        {
            return DeptCode != SavedDeptCode && DeptCode != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DeptCode);
        }

        public bool DeptName_Updated(Column column = null)
        {
            return DeptName != SavedDeptName && DeptName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != DeptName);
        }

        public bool Body_Updated(Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.DefaultInput.ToString() != Body);
        }

        public List<int> SwitchTargets;

        public DeptModel()
        {
        }

        public DeptModel(
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

        public DeptModel(
            SiteSettings ss,
            int deptId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            DeptId = deptId;
            Get(ss);
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm(ss);
            if (setByApi) SetByApi(ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public DeptModel(SiteSettings ss, DataRow dataRow, string tableAlias = null)
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

        public DeptModel Get(
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
            Set(ss, Rds.ExecuteTable(statements: Rds.SelectDepts(
                tableType: tableType,
                column: column ?? Rds.DeptsDefaultColumns(),
                join: join ??  Rds.DeptsJoinDefault(),
                where: where ?? Rds.DeptsWhereDefault(this),
                orderBy: orderBy,
                param: param,
                distinct: distinct,
                top: top)));
            return this;
        }

        public DeptApiModel GetByApi(SiteSettings ss)
        {
            var data = new DeptApiModel();
            ss.ReadableColumns(noJoined: true).ForEach(column =>
            {
                switch (column.ColumnName)
                {
                    case "TenantId": data.TenantId = TenantId; break;
                    case "DeptId": data.DeptId = DeptId; break;
                    case "Ver": data.Ver = Ver; break;
                    case "DeptCode": data.DeptCode = DeptCode; break;
                    case "DeptName": data.DeptName = DeptName; break;
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
            DeptId = newId != 0 ? newId : DeptId;
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
                Rds.InsertDepts(
                    tableType: tableType,
                        selectIdentity: true,
                    param: param ?? Rds.DeptsParamDefault(
                        this, setDefault: true, otherInitValue: otherInitValue)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.DeptsUpdated)
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
            SiteInfo.Reflesh();
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            List<SqlStatement> statements,
            DateTime timestamp,
            SqlParamCollection param,
            bool otherInitValue = false,
            List<SqlStatement> additionalStatements = null)
        {
            var where = Rds.DeptsWhereDefault(this)
                .UpdatedTime(timestamp, _using: timestamp.InRange());
            if (VerUp)
            {
                statements.Add(CopyToStatement(where, Sqls.TableTypes.History));
                Ver++;
            }
            statements.AddRange(new List<SqlStatement>
            {
                Rds.UpdateDepts(
                    where: where,
                    param: param ?? Rds.DeptsParamDefault(this, otherInitValue: otherInitValue),
                    countRecord: true),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.DeptsUpdated)
            });
            if (additionalStatements?.Any() == true)
            {
                statements.AddRange(additionalStatements);
            }
            return statements;
        }

        private SqlStatement CopyToStatement(SqlWhereCollection where, Sqls.TableTypes tableType)
        {
            var column = new Rds.DeptsColumnCollection();
            var param = new Rds.DeptsParamCollection();
            column.TenantId(function: Sqls.Functions.SingleColumn); param.TenantId();
            column.DeptId(function: Sqls.Functions.SingleColumn); param.DeptId();
            column.Ver(function: Sqls.Functions.SingleColumn); param.Ver();
            column.DeptCode(function: Sqls.Functions.SingleColumn); param.DeptCode();
            column.DeptName(function: Sqls.Functions.SingleColumn); param.DeptName();
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
            return Rds.InsertDepts(
                tableType: tableType,
                param: param,
                select: Rds.SelectDepts(column: column, where: where),
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
                Rds.UpdateOrInsertDepts(
                    selectIdentity: true,
                    where: where ?? Rds.DeptsWhereDefault(this),
                    param: param ?? Rds.DeptsParamDefault(this, setDefault: true)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.DeptsUpdated)
            };
            var newId = Rds.ExecuteScalar_int(
                rdsUser: rdsUser,
                transactional: true,
                statements: statements.ToArray());
            DeptId = newId != 0 ? newId : DeptId;
            Get(ss);
            return Error.Types.None;
        }

        public Error.Types Delete(SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.DeptsWhere().DeptId(DeptId);
            statements.AddRange(new List<SqlStatement>
            {
                CopyToStatement(where, Sqls.TableTypes.Deleted),
                Rds.PhysicalDeleteDepts(where: where),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.DeptsUpdated)
            });
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: statements.ToArray());
            var deptHash = SiteInfo.TenantCaches[Sessions.TenantId()].DeptHash;
            if (deptHash.Keys.Contains(DeptId))
            {
                deptHash.Remove(DeptId);
            }
            return Error.Types.None;
        }

        public Error.Types Restore(SiteSettings ss,int deptId)
        {
            DeptId = deptId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreDepts(
                        where: Rds.DeptsWhere().DeptId(DeptId)),
                StatusUtilities.UpdateStatus(StatusUtilities.Types.DeptsUpdated)
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteDepts(
                    tableType: tableType,
                    param: Rds.DeptsParam().DeptId(DeptId)));
            return Error.Types.None;
        }

        public void SetByForm(SiteSettings ss)
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Depts_DeptCode": DeptCode = Forms.Data(controlId).ToString(); break;
                    case "Depts_DeptName": DeptName = Forms.Data(controlId).ToString(); break;
                    case "Depts_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Depts_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
            var data = Forms.String().Deserialize<DeptApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.DeptCode != null) DeptCode = data.DeptCode.ToString().ToString();
            if (data.DeptName != null) DeptName = data.DeptName.ToString().ToString();
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
                        case "DeptId":
                            if (dataRow[column.ColumnName] != DBNull.Value)
                            {
                                DeptId = dataRow[column.ColumnName].ToInt();
                                SavedDeptId = DeptId;
                            }
                            break;
                        case "Ver":
                            Ver = dataRow[column.ColumnName].ToInt();
                            SavedVer = Ver;
                            break;
                        case "DeptCode":
                            DeptCode = dataRow[column.ColumnName].ToString();
                            SavedDeptCode = DeptCode;
                            break;
                        case "DeptName":
                            DeptName = dataRow[column.ColumnName].ToString();
                            SavedDeptName = DeptName;
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
                DeptId_Updated() ||
                Ver_Updated() ||
                DeptCode_Updated() ||
                DeptName_Updated() ||
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToControl(SiteSettings ss, Column column)
        {
            return string.Empty;
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
            return hb.Td(action: () => hb
                .HtmlDept(DeptId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToExport(Column column, ExportColumn exportColumn = null)
        {
            return DeptName;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool InitialValue()
        {
            return DeptId == 0;
        }
    }
}
