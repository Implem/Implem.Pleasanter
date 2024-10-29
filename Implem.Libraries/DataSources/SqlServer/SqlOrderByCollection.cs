using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
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
            string columnBracket = null,
            SqlOrderBy.Types orderType = SqlOrderBy.Types.asc,
            string tableName = null,
            string isNullValue = null,
            Sqls.Functions function = Sqls.Functions.None,
            SqlStatement sub = null,
            string raw = null)
        {
            if (orderType != SqlOrderBy.Types.release)
            {
                Add(new SqlOrderBy(
                    columnBracket: columnBracket,
                    orderType: orderType,
                    tableName: tableName,
                    isNullValue: isNullValue,
                    function: function,
                    sub: sub,
                    raw: raw));
            }
            return this;
        }

        public void BuildCommandText(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
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
                        .GroupBy(o => $"{o.TableName}{o.ColumnBracket}" == string.Empty
                            ? Strings.NewGuid()
                            : $"{o.TableName}.{o.ColumnBracket}")
                        .Select(o => o.FirstOrDefault())
                        .Select(o => o.Sql(
                            factory: factory,
                            sqlContainer: sqlContainer,
                            sqlCommand: sqlCommand,
                            tableBracket: Sqls.GetTableBracket(o.TableName),
                            tableType: tableType))
                        .Join(),
                    " ");
                if (pageSize != 0)
                {
                    commandText.Append(factory.SqlCommandText.CreateDataRangeCommand(commandCount));
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
