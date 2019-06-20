using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.SqlServer;
using Implem.Libraries.Utilities;
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
        public int TotalCount;

        public PermissionCollection(
            Context context,
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
            bool get = true)
        {
            if (get)
            {
                Set(
                    context: context,
                    dataRows: Get(
                        context: context,
                        column: column,
                        join: join,
                        where: where,
                        orderBy: orderBy,
                        param: param,
                        tableType: tableType,
                        distinct: distinct,
                        top: top,
                        offset: offset,
                        pageSize: pageSize));
            }
        }

        public PermissionCollection(
            Context context,
            EnumerableRowCollection<DataRow> dataRows)
        {
                Set(
                    context: context,
                    dataRows: dataRows);
        }

        private PermissionCollection Set(
            Context context,
            EnumerableRowCollection<DataRow> dataRows)
        {
            if (dataRows.Any())
            {
                foreach (DataRow dataRow in dataRows)
                {
                    Add(new PermissionModel(
                        context: context,
                        dataRow: dataRow));
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
            Context context,
            SqlColumnCollection column = null,
            SqlJoinCollection join = null,
            SqlWhereCollection where = null,
            SqlOrderByCollection orderBy = null,
            SqlParamCollection param = null,
            Sqls.TableTypes tableType = Sqls.TableTypes.Normal,
            bool distinct = false,
            int top = 0,
            int offset = 0,
            int pageSize = 0)
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
                    pageSize: pageSize),
                Rds.SelectCount(
                    tableName: "Permissions",
                    tableType: tableType,
                    join: join ?? Rds.PermissionsJoinDefault(),
                    where: where)
            };
            var dataSet = Rds.ExecuteDataSet(
                context: context,
                transactional: false,
                statements: statements.ToArray());
            TotalCount = Rds.Count(dataSet);
            return dataSet.Tables["Main"].AsEnumerable();
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public PermissionCollection(
            Context context, long referenceId, IEnumerable<string> permissions)
        {
            permissions?.ForEach(line =>
            {
                var parts = line.Split(',');
                if (parts.Count() == 3)
                {
                    Add(new PermissionModel(
                        context: context,
                        referenceId: referenceId,
                        deptId: parts[0] == "Dept" ? parts[1].ToInt() : 0,
                        groupId: parts[0] == "Group" ? parts[1].ToInt() : 0,
                        userId: parts[0] == "User" ? parts[1].ToInt() : 0,
                        permissionType: (Permissions.Types)parts[2].ToLong()));
                }
            });
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        public bool InTenant(Context context)
        {
            var depts = this.Where(o => o.DeptId > 0).Select(o => o.DeptId);
            var groups = this.Where(o => o.GroupId > 0).Select(o => o.GroupId);
            var users = this.Where(o => o.UserId > 0).Select(o => o.UserId);
            var dataSet = Rds.ExecuteDataSet(
                context: context,
                statements: new SqlStatement[]
                {
                    Rds.SelectDepts(
                        dataTableName: "Depts",
                        column: Rds.DeptsColumn().DeptId(),
                        where: Rds.DeptsWhere()
                            .TenantId(context.TenantId)
                            .DeptId_In(depts)),
                    Rds.SelectGroups(
                        dataTableName: "Groups",
                        column: Rds.GroupsColumn().GroupId(),
                        where: Rds.GroupsWhere()
                            .TenantId(context.TenantId)
                            .GroupId_In(groups)),
                    Rds.SelectUsers(
                        dataTableName: "Users",
                        column: Rds.UsersColumn().UserId(),
                        where: Rds.UsersWhere()
                            .TenantId(context.TenantId)
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
