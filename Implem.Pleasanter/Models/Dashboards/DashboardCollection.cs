﻿using Implem.DefinitionAccessor;
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
    public class DashboardCollection : List<DashboardModel>
    {
        [NonSerialized]
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public int TotalCount;
        public DashboardCollection(
            Context context,
            SiteSettings ss,
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
            bool get = true,
            List<FormData> formDataSet = null)
        {
            if (get)
            {
                Set(
                    context: context,
                    ss: ss,
                    dataRows: Get(
                        context: context,
                        ss: ss,
                        column: column,
                        join: join,
                        where: where,
                        orderBy: orderBy,
                        param: param,
                        tableType: tableType,
                        distinct: distinct,
                        top: top,
                        offset: offset,
                        pageSize: pageSize),
                    formDataSet: formDataSet);
            }
        }
        public DashboardCollection(
            Context context,
            SiteSettings ss,
            EnumerableRowCollection<DataRow> dataRows,
            List<FormData> formDataSet = null)
        {
                Set(
                    context: context,
                    ss: ss,
                    dataRows: dataRows,
                    formDataSet: formDataSet);
        }
        private DashboardCollection Set(
            Context context,
            SiteSettings ss,
            EnumerableRowCollection<DataRow> dataRows,
            List<FormData> formDataSet = null)
        {
            if (dataRows.Any())
            {
                foreach (DataRow dataRow in dataRows)
                {
                    Add(new DashboardModel(
                        context: context,
                        ss: ss,
                        dataRow: dataRow,
                        formData: formDataSet?.FirstOrDefault(o =>
                            o.Id == dataRow.Long("DashboardId"))?.Data));
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
            SiteSettings ss,
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
                Rds.SelectDashboards(
                    dataTableName: "Main",
                    column: column ?? Rds.DashboardsDefaultColumns(),
                    join: join ??  Rds.DashboardsJoinDefault(),
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize),
                Rds.SelectCount(
                    tableName: "Dashboards",
                    tableType: tableType,
                    join: join ?? Rds.DashboardsJoinDefault(),
                    where: where)
            };
            var dataSet = Repository.ExecuteDataSet(
                context: context,
                transactional: false,
                statements: statements.ToArray());
            TotalCount = Rds.Count(dataSet);
            return dataSet.Tables["Main"].AsEnumerable();
        }
    }
}
