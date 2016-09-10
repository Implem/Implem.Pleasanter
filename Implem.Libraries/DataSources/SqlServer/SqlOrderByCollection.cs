using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlOrderByCollection : ListEx<SqlOrderBy>
    {
        public SqlOrderByCollection(params SqlOrderBy[] sqlOrderByCollection)
        {
            this.AddRange(sqlOrderByCollection.Where(o =>
                o.OrderType != SqlOrderBy.Types.release));
        }

        public SqlOrderByCollection Add(SqlOrderBy.Types type, params string[] columnBrackets)
        {
            if (type != SqlOrderBy.Types.release)
            {
                columnBrackets.ForEach(columnBracket =>
                    base.Add(new SqlOrderBy(columnBracket, type)));
            }
            return this;
        }

        public void BuildCommandText(StringBuilder commandText, int pageSize, int? commandCount)
        {
            if (this.Count > 0)
            {
                var orderBy = new Dictionary<string, string>();
                this.ForEach(o =>
                {
                    if (!orderBy.ContainsKey(o.ColumnBracket))
                    {
                        orderBy.Add(o.ColumnBracket, o.ColumnBracket + " " + o.OrderType);
                    }
                });
                commandText.Append("order by ", orderBy.Values.Join(), " ");
                if (pageSize != 0)
                {
                    commandText.Append(
                        "offset @_Offset", commandCount.ToString(),
                        " rows fetch next @_PageSize", commandCount.ToString(),
                        " rows only ");
                }
            }
        }
    }
}
