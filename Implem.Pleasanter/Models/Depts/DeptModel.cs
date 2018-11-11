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
        public int TenantId = 0;
        public int DeptId = 0;
        public string DeptCode = string.Empty;
        public string DeptName = string.Empty;
        public string Body = string.Empty;

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
                return new Title(DeptId, DeptName);
            }
        }

        [NonSerialized] public int SavedTenantId = 0;
        [NonSerialized] public int SavedDeptId = 0;
        [NonSerialized] public string SavedDeptCode = string.Empty;
        [NonSerialized] public string SavedDeptName = string.Empty;
        [NonSerialized] public string SavedBody = string.Empty;

        public bool TenantId_Updated(Context context, Column column = null)
        {
            return TenantId != SavedTenantId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != TenantId);
        }

        public bool DeptId_Updated(Context context, Column column = null)
        {
            return DeptId != SavedDeptId &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToInt() != DeptId);
        }

        public bool DeptCode_Updated(Context context, Column column = null)
        {
            return DeptCode != SavedDeptCode && DeptCode != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DeptCode);
        }

        public bool DeptName_Updated(Context context, Column column = null)
        {
            return DeptName != SavedDeptName && DeptName != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != DeptName);
        }

        public bool Body_Updated(Context context, Column column = null)
        {
            return Body != SavedBody && Body != null &&
                (column == null ||
                column.DefaultInput.IsNullOrEmpty() ||
                column.GetDefaultInput(context: context).ToString() != Body);
        }

        public List<int> SwitchTargets;

        public DeptModel()
        {
        }

        public DeptModel(
            Context context,
            SiteSettings ss,
            bool setByForm = false,
            bool setByApi = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            if (setByForm) SetByForm(context: context, ss: ss);
            if (setByApi) SetByApi(context: context, ss: ss);
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public DeptModel(
            Context context,
            SiteSettings ss,
            int deptId,
            bool clearSessions = false,
            bool setByForm = false,
            bool setByApi = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            DeptId = deptId;
            Get(context: context, ss: ss);
            if (clearSessions) ClearSessions(context: context);
            if (setByForm) SetByForm(context: context, ss: ss);
            if (setByApi) SetByApi(context: context, ss: ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed(context: context);
        }

        public DeptModel(Context context, SiteSettings ss, DataRow dataRow, string tableAlias = null)
        {
            OnConstructing(context: context);
            Context = context;
            TenantId = context.TenantId;
            if (dataRow != null) Set(context, ss, dataRow, tableAlias);
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

        public DeptModel Get(
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
            Set(context, ss, Rds.ExecuteTable(
                context: context,
                statements: Rds.SelectDepts(
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

        public DeptApiModel GetByApi(Context context, SiteSettings ss)
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
                    case "CreatedTime": data.CreatedTime = CreatedTime.Value.ToLocal(context: context); break;
                    case "UpdatedTime": data.UpdatedTime = UpdatedTime.Value.ToLocal(context: context); break;
                    case "Comments": data.Comments = Comments.ToLocal(context: context).ToJson(); break;
                }
            });
            return data;
        }

        public Error.Types Create(
            Context context,
            SiteSettings ss,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false,
            bool get = true)
        {
            TenantId = context.TenantId;
            var statements = new List<SqlStatement>();
            CreateStatements(context, ss, statements, tableType, param, otherInitValue);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            DeptId = (response.Identity ?? DeptId).ToInt();
            if (get) Get(context: context, ss: ss);
            return Error.Types.None;
        }

        public List<SqlStatement> CreateStatements(
            Context context,
            SiteSettings ss,
            List<SqlStatement> statements,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool otherInitValue = false)
        {
            statements.AddRange(new List<SqlStatement>
            {
                Rds.InsertDepts(
                    tableType: tableType,
                    setIdentity: true,
                    param: param ?? Rds.DeptsParamDefault(
                        context: context,
                        deptModel: this,
                        setDefault: true,
                        otherInitValue: otherInitValue)),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.DeptsUpdated),
            });
            return statements;
        }

        public Error.Types Update(
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
            if (setBySession) SetBySession(context: context);
            var timestamp = Timestamp.ToDateTime();
            var statements = new List<SqlStatement>();
            UpdateStatements(
                context: context,
                ss: ss,
                statements: statements,
                timestamp: timestamp,
                param: param,
                otherInitValue: otherInitValue,
                additionalStatements: additionalStatements);
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            if (response.Count == 0) return Error.Types.UpdateConflicts;
            if (get) Get(context: context, ss: ss);
            SiteInfo.Reflesh(context: context);
            return Error.Types.None;
        }

        private List<SqlStatement> UpdateStatements(
            Context context,
            SiteSettings ss,
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
                    param: param ?? Rds.DeptsParamDefault(
                        context: context, deptModel: this, otherInitValue: otherInitValue),
                    countRecord: true),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.DeptsUpdated),
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
            column.Body(function: Sqls.Functions.SingleColumn); param.Body();
            column.Comments(function: Sqls.Functions.SingleColumn); param.Comments();
            column.Creator(function: Sqls.Functions.SingleColumn); param.Creator();
            column.Updator(function: Sqls.Functions.SingleColumn); param.Updator();
            column.CreatedTime(function: Sqls.Functions.SingleColumn); param.CreatedTime();
            column.UpdatedTime(function: Sqls.Functions.SingleColumn); param.UpdatedTime();
            return Rds.InsertDepts(
                tableType: tableType,
                param: param,
                select: Rds.SelectDepts(column: column, where: where),
                addUpdatorParam: false);
        }

        public Error.Types UpdateOrCreate(
            Context context,
            SiteSettings ss,
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession(context: context);
            var statements = new List<SqlStatement>
            {
                Rds.UpdateOrInsertDepts(
                    where: where ?? Rds.DeptsWhereDefault(this),
                    param: param ?? Rds.DeptsParamDefault(
                        context: context, deptModel: this, setDefault: true)),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.DeptsUpdated),
            };
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                selectIdentity: true,
                statements: statements.ToArray());
            DeptId = (response.Identity ?? DeptId).ToInt();
            Get(context: context, ss: ss);
            return Error.Types.None;
        }

        public Error.Types Delete(Context context, SiteSettings ss, bool notice = false)
        {
            var statements = new List<SqlStatement>();
            var where = Rds.DeptsWhere().DeptId(DeptId);
            statements.AddRange(new List<SqlStatement>
            {
                Rds.DeleteDepts(where: where),
                StatusUtilities.UpdateStatus(
                    tenantId: context.TenantId,
                    type: StatusUtilities.Types.DeptsUpdated),
            });
            var response = Rds.ExecuteScalar_response(
                context: context,
                transactional: true,
                statements: statements.ToArray());
            var deptHash = SiteInfo.TenantCaches.Get(context.TenantId)?.DeptHash;
            if (deptHash.Keys.Contains(DeptId))
            {
                deptHash.Remove(DeptId);
            }
            return Error.Types.None;
        }

        public Error.Types Restore(Context context, SiteSettings ss,int deptId)
        {
            DeptId = deptId;
            Rds.ExecuteNonQuery(
                context: context,
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreDepts(
                        where: Rds.DeptsWhere().DeptId(DeptId)),
                    StatusUtilities.UpdateStatus(
                        tenantId: context.TenantId,
                        type: StatusUtilities.Types.DeptsUpdated),
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            Context context, SiteSettings ss,Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                context: context,
                transactional: true,
                statements: Rds.PhysicalDeleteDepts(
                    tableType: tableType,
                    param: Rds.DeptsParam().DeptId(DeptId)));
            return Error.Types.None;
        }

        public void SetByForm(Context context, SiteSettings ss)
        {
            context.Forms.Keys.ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Depts_DeptCode": DeptCode = context.Forms.Data(controlId).ToString(); break;
                    case "Depts_DeptName": DeptName = context.Forms.Data(controlId).ToString(); break;
                    case "Depts_Body": Body = context.Forms.Data(controlId).ToString(); break;
                    case "Depts_Timestamp": Timestamp = context.Forms.Data(controlId).ToString(); break;
                    case "Comments": Comments.Prepend(context: context, ss: ss, body: context.Forms.Data("Comments")); break;
                    case "VerUp": VerUp = context.Forms.Data(controlId).ToBool(); break;
                    default:
                        if (controlId.RegexExists("Comment[0-9]+"))
                        {
                            Comments.Update(
                                context: context,
                                ss: ss,
                                commentId: controlId.Substring("Comment".Length).ToInt(),
                                body: context.Forms.Data(controlId));
                        }
                        break;
                }
            });
            if (context.Action == "deletecomment")
            {
                DeleteCommentId = context.Forms.ControlId().Split(',')._2nd().ToInt();
                Comments.RemoveAll(o => o.CommentId == DeleteCommentId);
            }
        }

        public void SetByModel(DeptModel deptModel)
        {
            TenantId = deptModel.TenantId;
            DeptCode = deptModel.DeptCode;
            DeptName = deptModel.DeptName;
            Body = deptModel.Body;
            Comments = deptModel.Comments;
            Creator = deptModel.Creator;
            Updator = deptModel.Updator;
            CreatedTime = deptModel.CreatedTime;
            UpdatedTime = deptModel.UpdatedTime;
            VerUp = deptModel.VerUp;
            Comments = deptModel.Comments;
        }

        public void SetByApi(Context context, SiteSettings ss)
        {
            var data = context.FormString.Deserialize<DeptApiModel>();
            if (data == null)
            {
                return;
            }
            if (data.DeptCode != null) DeptCode = data.DeptCode.ToString().ToString();
            if (data.DeptName != null) DeptName = data.DeptName.ToString().ToString();
            if (data.Body != null) Body = data.Body.ToString().ToString();
            if (data.Comments != null) Comments.Prepend(context: context, ss: ss, body: data.Comments);
            if (data.VerUp != null) VerUp = data.VerUp.ToBool();
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
                        case "IsHistory": VerType = dataRow[column.ColumnName].ToBool() ? Versions.VerTypes.History : Versions.VerTypes.Latest; break;
                    }
                }
            }
        }

        public bool Updated(Context context)
        {
            return
                TenantId_Updated(context: context) ||
                DeptId_Updated(context: context) ||
                Ver_Updated(context: context) ||
                DeptCode_Updated(context: context) ||
                DeptName_Updated(context: context) ||
                Body_Updated(context: context) ||
                Comments_Updated(context: context) ||
                Creator_Updated(context: context) ||
                Updator_Updated(context: context);
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
        public string ToControl(Context context, SiteSettings ss, Column column)
        {
            return string.Empty;
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
            return hb.Td(action: () => hb
                .HtmlDept(
                    context: context,
                    id: DeptId));
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToExport(Context context, Column column, ExportColumn exportColumn = null)
        {
            return DeptName;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool InitialValue(Context context)
        {
            return DeptId == 0;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public System.Web.Mvc.ContentResult GetByApi(Context context)
        {
            return DeptUtilities.GetByApi(
                context: context,
                ss: SiteSettingsUtilities.ApiDeptsSiteSettings(context));
        }
    }
}
