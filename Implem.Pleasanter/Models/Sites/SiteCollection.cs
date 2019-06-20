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
    public class SiteCollection : List<SiteModel>
    {
        [NonSerialized]
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public int TotalCount;
        public SiteCollection(
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
            bool get = true,
            List<FormData> formDataSet = null)
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
                        pageSize: pageSize),
                    formDataSet: formDataSet);
            }
        }
        public SiteCollection(
            Context context,
            EnumerableRowCollection<DataRow> dataRows,
            List<FormData> formDataSet = null)
        {
                Set(
                    context: context,
                    dataRows: dataRows,
                    formDataSet: formDataSet);
        }
        private SiteCollection Set(
            Context context,
            EnumerableRowCollection<DataRow> dataRows,
            List<FormData> formDataSet = null)
        {
            if (dataRows.Any())
            {
                foreach (DataRow dataRow in dataRows)
                {
                    Add(new SiteModel(
                        context: context,
                        dataRow: dataRow,
                        formData: formDataSet?.FirstOrDefault(o =>
                            o.Id == dataRow.Long("SiteId"))?.Data));
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
                Rds.SelectSites(
                    dataTableName: "Main",
                    column: column ?? Rds.SitesDefaultColumns(),
                    join: join ??  Rds.SitesJoinDefault(),
                    where: where,
                    orderBy: orderBy,
                    param: param,
                    tableType: tableType,
                    distinct: distinct,
                    top: top,
                    offset: offset,
                    pageSize: pageSize),
                Rds.SelectCount(
                    tableName: "Sites",
                    tableType: tableType,
                    join: join ?? Rds.SitesJoinDefault(),
                    where: where)
            };
            var dataSet = Rds.ExecuteDataSet(
                context: context,
                transactional: false,
                statements: statements.ToArray());
            TotalCount = Rds.Count(dataSet);
            return dataSet.Tables["Main"].AsEnumerable();
        }
    }
}
