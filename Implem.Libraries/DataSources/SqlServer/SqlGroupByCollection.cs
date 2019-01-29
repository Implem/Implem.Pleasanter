using Implem.Libraries.Classes;
using Implem.Libraries.DataSources.Interfaces;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlGroupByCollection : ListEx<SqlGroupBy>, IJoin
    {
        public SqlGroupByCollection(params SqlGroupBy[] sqlGroupByCollection)
        {
            AddRange(sqlGroupByCollection);
        }

        public SqlGroupByCollection Add(string columnBracket, string tableName)
        {
            Add(new SqlGroupBy(columnBracket, tableName));
            return this;
        }

        public void BuildCommandText(StringBuilder commandText)
        {
            if (this.Any())
            {
                commandText.Append("group by ", this.Select(o =>
                    Sqls.TableAndColumnBracket(
                        tableBracket: Sqls.GetTableBracket(o.TableName),
                        columnBracket: o.ColumnBracket))
                            .Join(), " ");
            }
        }

        public List<string> JoinTableNames()
        {
            return this
                .Select(o => o.TableName)
                .Where(o => o?.Contains("~") == true)
                .Distinct()
                .ToList();
        }
    }
}
