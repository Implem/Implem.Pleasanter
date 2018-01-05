using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Converts;
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
    [Serializable]
    public class PermissionCollection : List<PermissionModel>
    {
        [NonSerialized]
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;

        public PermissionCollection(
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool countRecord = false,
            bool get = true)
        {
            if (get)
            {
                Set(Get(
                    column: column,
                    join: join,
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord));
            }
        }

        public PermissionCollection(EnumerableRowCollection<DataRow> dataRows)
        {
            Set(dataRows);
        }

        private PermissionCollection Set(EnumerableRowCollection<DataRow> dataRows)
        {
            if (dataRows.Any())
            {
                foreach (DataRow dataRow in dataRows)
                {
                    Add(new PermissionModel(dataRow));
                }
                AccessStatus = Databases.AccessStatuses.Selected;
            }
            else
            {
                AccessStatus = Databases.AccessStatuses.NotFound;
            }
            return this;
        }

        private EnumerableRowCollection<DataRow> Get(
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0,
            bool history = false,
            bool countRecord = false)
        {
            var statements = new List<SqlStatement>
            {
                Rds.SelectPermissions(
                    dataTableName: "Main",
                    column: column ?? Rds.PermissionsDefaultColumns(),
                    join: join ??  Rds.PermissionsJoinDefault(),
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize,
                    countRecord: countRecord)
            };
            var dataSet = Rds.ExecuteDataSet(
                transactional: false,
                statements: statements.ToArray());
            return dataSet.Tables["Main"].AsEnumerable();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public PermissionCollection(long referenceId, IEnumerable<string> permissions)
        {
            permissions?.ForEach(line =>
            {
                var parts = line.Split(',');
                if (parts.Count() == 3)
                {
                    Add(new PermissionModel(
                        referenceId,
                        parts[0] == "Dept" ? parts[1].ToInt() : 0,
                        parts[0] == "Group" ? parts[1].ToInt() : 0,
                        parts[0] == "User" ? parts[1].ToInt() : 0,
                        (Permissions.Types)parts[2].ToLong()));
                }
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool InTenant()
        {
            var tenantId = Sessions.TenantId();
            var depts = this.Where(o => o.DeptId > 0).Select(o => o.DeptId);
            var groups = this.Where(o => o.GroupId > 0).Select(o => o.GroupId);
            var users = this.Where(o => o.UserId > 0).Select(o => o.UserId);
            var dataSet = Rds.ExecuteDataSet(statements: new SqlStatement[]
            {
                Rds.SelectDepts(
                    dataTableName: "Depts",
                    column: Rds.DeptsColumn().DeptId(),
                    where: Rds.DeptsWhere()
                        .TenantId(tenantId)
                        .DeptId_In(depts)),
                Rds.SelectGroups(
                    dataTableName: "Groups",
                    column: Rds.GroupsColumn().GroupId(),
                    where: Rds.GroupsWhere()
                        .TenantId(tenantId)
                        .GroupId_In(groups)),
                Rds.SelectUsers(
                    dataTableName: "Users",
                    column: Rds.UsersColumn().UserId(),
                    where: Rds.UsersWhere()
                        .TenantId(tenantId)
                        .UserId_In(users))
            });
            var deptRecords = dataSet.Tables["Depts"]
                .AsEnumerable()
                .Select(p => p["DeptId"].ToInt());
            var groupRecords = dataSet.Tables["Groups"]
                .AsEnumerable()
                .Select(p => p["GroupId"].ToInt());
            var userRecords = dataSet.Tables["Users"]
                .AsEnumerable()
                .Select(p => p["UserId"].ToInt());
            return
                depts.All(o => deptRecords.Contains(o)) ||
                groups.All(o => groupRecords.Contains(o)) ||
                users.All(o => userRecords.Contains(o));
        }
    }
}
