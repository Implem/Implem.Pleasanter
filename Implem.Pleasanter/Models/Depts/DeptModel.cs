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
    public class DeptModel : BaseModel, Interfaces.IConvertable
    {
        public int TenantId = Sessions.TenantId();
        public int DeptId = 0;
        public string DeptCode = string.Empty;
        public string DeptName = string.Empty;
        public string Body = string.Empty;
        public Dept Dept { get { return SiteInfo.Dept(DeptId); } }
        public Title Title { get { return new Title(DeptId, DeptName); } }
        public int SavedTenantId = Sessions.TenantId();
        public int SavedDeptId = 0;
        public string SavedDeptCode = string.Empty;
        public string SavedDeptName = string.Empty;
        public string SavedBody = string.Empty;
        public bool TenantId_Updated { get { return TenantId != SavedTenantId; } }
        public bool DeptId_Updated { get { return DeptId != SavedDeptId; } }
        public bool DeptCode_Updated { get { return DeptCode != SavedDeptCode && DeptCode != null; } }
        public bool DeptName_Updated { get { return DeptName != SavedDeptName && DeptName != null; } }
        public bool Body_Updated { get { return Body != SavedBody && Body != null; } }
        public List<int> SwitchTargets;

        public DeptModel()
        {
        }

        public DeptModel(
            SiteSettings ss, 
            bool setByForm = false,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            if (setByForm) SetByForm(ss);
            MethodType = methodType;
            OnConstructed();
        }

        public DeptModel(
            SiteSettings ss, 
            int deptId,
            bool clearSessions = false,
            bool setByForm = false,
            List<int> switchTargets = null,
            MethodTypes methodType = MethodTypes.NotSet)
        {
            OnConstructing();
            DeptId = deptId;
            Get(ss);
            if (clearSessions) ClearSessions();
            if (setByForm) SetByForm(ss);
            SwitchTargets = switchTargets;
            MethodType = methodType;
            OnConstructed();
        }

        public DeptModel(SiteSettings ss, DataRow dataRow)
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
                orderBy: orderBy ?? null,
                param: param ?? null,
                distinct: distinct,
                top: top)));
            return this;
        }

        public Error.Types Create(
            SiteSettings ss, 
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            SqlParamCollection param = null,
            bool paramAll = false)
        {
            var newId = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.InsertDepts(
                        tableType: tableType,
                        selectIdentity: true,
                        param: param ?? Rds.DeptsParamDefault(
                            this, setDefault: true, paramAll: paramAll))
                });
            DeptId = newId != 0 ? newId : DeptId;
            Get(ss);
            return Error.Types.None;
        }

        public Error.Types Update(SiteSettings ss, bool notice = false,bool paramAll = false)
        {
            SetBySession();
            var timestamp = Timestamp.ToDateTime();
            var count = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateDepts(
                        verUp: VerUp,
                        where: Rds.DeptsWhereDefault(this)
                            .UpdatedTime(timestamp, _using: timestamp.InRange()),
                        param: Rds.DeptsParamDefault(this, paramAll: paramAll),
                        countRecord: true)
                });
            if (count == 0) return Error.Types.UpdateConflicts;
            Get(ss);
            SiteInfo.Reflesh();
            return Error.Types.None;
        }

        public Error.Types UpdateOrCreate(
            SiteSettings ss, 
            SqlWhereCollection where = null,
            SqlParamCollection param = null)
        {
            SetBySession();
            var newId = Rds.ExecuteScalar_int(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.UpdateOrInsertDepts(
                        selectIdentity: true,
                        where: where ?? Rds.DeptsWhereDefault(this),
                        param: param ?? Rds.DeptsParamDefault(this, setDefault: true))
                });
            DeptId = newId != 0 ? newId : DeptId;
            Get(ss);
            return Error.Types.None;
        }

        public Error.Types Delete(SiteSettings ss, bool notice = false)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.DeleteDepts(
                        where: Rds.DeptsWhere().DeptId(DeptId))
                });
            if (SiteInfo.DeptHash.Keys.Contains(DeptId))
            {
                SiteInfo.DeptHash.Remove(DeptId);
            }
            return Error.Types.None;
        }

        public Error.Types Restore(SiteSettings ss, int deptId)
        {
            DeptId = deptId;
            Rds.ExecuteNonQuery(
                connectionString: Parameters.Rds.OwnerConnectionString,
                transactional: true,
                statements: new SqlStatement[]
                {
                    Rds.RestoreDepts(
                        where: Rds.DeptsWhere().DeptId(DeptId))
                });
            return Error.Types.None;
        }

        public Error.Types PhysicalDelete(
            SiteSettings ss, Sqls.TableTypes tableType = Sqls.TableTypes.Normal)
        {
            Rds.ExecuteNonQuery(
                transactional: true,
                statements: Rds.PhysicalDeleteDepts(
                    tableType: tableType,
                    param: Rds.DeptsParam().DeptId(DeptId)));
            return Error.Types.None;
        }

        private void SetByForm(SiteSettings ss)
        {
            Forms.Keys().ForEach(controlId =>
            {
                switch (controlId)
                {
                    case "Depts_DeptCode": DeptCode = Forms.Data(controlId).ToString(); break;
                    case "Depts_DeptName": DeptName = Forms.Data(controlId).ToString(); break;
                    case "Depts_Body": Body = Forms.Data(controlId).ToString(); break;
                    case "Depts_Timestamp": Timestamp = Forms.Data(controlId).ToString(); break;
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
                    case "DeptId": if (dataRow[name] != DBNull.Value) { DeptId = dataRow[name].ToInt(); SavedDeptId = DeptId; } break;
                    case "Ver": Ver = dataRow[name].ToInt(); SavedVer = Ver; break;
                    case "DeptCode": DeptCode = dataRow[name].ToString(); SavedDeptCode = DeptCode; break;
                    case "DeptName": DeptName = dataRow[name].ToString(); SavedDeptName = DeptName; break;
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

        /// <summary>
        /// Fixed:
        /// </summary>
        public string ToControl(Column column, SiteSettings ss)
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
        public string ToExport(Column column)
        {
            return DeptName;
        }
    }
}
