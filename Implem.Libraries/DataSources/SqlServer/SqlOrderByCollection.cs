using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlOrderByCollection : ListEx<SqlOrderBy>, IJoin
    {
        public SqlOrderByCollection(params SqlOrderBy[] sqlOrderByCollection)
        {
            AddRange(sqlOrderByCollection.Where(o =>
                o.OrderType != SqlOrderBy.Types.release));
        }

        public SqlOrderByCollection Add(
            string columnBracket,
            SqlOrderBy.Types orderType,
            string tableName,
            Sqls.Functions function = Sqls.Functions.None)
        {
            if (orderType != SqlOrderBy.Types.release)
            {
                Add(new SqlOrderBy(
                    columnBracket: columnBracket,
                    orderType: orderType,
                    tableName: tableName,
                    function: function));
            }
            return this;
        }

        public void BuildCommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int pageSize,
            Sqls.TableTypes tableType,
            int? commandCount)
        {
            if (Count > 0)
            {
                commandText.Append(
                    "order by ",
                    this
                        .GroupBy(o => o.ColumnBracket)
                        .Select(o => o.FirstOrDefault())
                        .Select(o => o.Sql(
                            sqlContainer: sqlContainer,
                            sqlCommand: sqlCommand,
                            tableBracket: Sqls.GetTableBracket(o.TableName),
                            tableType: tableType))
                        .Join(),
                    " ");
                if (pageSize != 0)
                {
                    commandText.Append(
                        "offset @_Offset", commandCount.ToString(),
                        " rows fetch next @_PageSize", commandCount.ToString(),
                        " rows only ");
                }
            }
        }

        public List<string> JoinTableNames()
        {
            return this
                .Select(o => o.TableName.CutEnd("_Items"))
                .Where(o => o?.Contains("~") == true)
                .Distinct()
                .ToList();
        }
    }
}
