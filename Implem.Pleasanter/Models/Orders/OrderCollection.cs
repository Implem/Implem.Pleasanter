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
    public class OrderCollection : List<OrderModel>
    {
        [NonSerialized]
        public Databases.AccessStatuses AccessStatus = Databases.AccessStatuses.Initialized;
        public int TotalCount;

        public OrderCollection(
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
            bool countRecord = false,
            bool get = true)
        {
            if (get)
            {
                Set(context, Get(
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
                    pageSize: pageSize,
                    countRecord: countRecord));
            }
        }

        public OrderCollection(Context context,EnumerableRowCollection<DataRow> dataRows)
        {
            Set(context, dataRows);
        }

        private OrderCollection Set(Context context, EnumerableRowCollection<DataRow> dataRows)
        {
            if (dataRows.Any())
            {
                foreach (DataRow dataRow in dataRows)
                {
                    Add(new OrderModel(context, dataRow));
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
            int pageSize = 0,
            bool history = false,
            bool countRecord = false)
        {
            var statements = new List<SqlStatement>
            {
                Rds.SelectOrders(
                    dataTableName: "Main",
                    column: column ?? Rds.OrdersDefaultColumns(),
                    join: join ??  Rds.OrdersJoinDefault(),
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
                context: context,
                transactional: false,
                statements: statements.ToArray());
            TotalCount = Rds.Count(dataSet);
            return dataSet.Tables["Main"].AsEnumerable();
        }
    }
}
