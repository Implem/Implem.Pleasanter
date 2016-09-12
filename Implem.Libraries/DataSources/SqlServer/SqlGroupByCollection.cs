using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlGroupByCollection : ListEx<SqlGroupBy>
    {
        public SqlGroupByCollection(params SqlGroupBy[] sqlGroupByCollection)
        {
            this.AddRange(sqlGroupByCollection);
        }

        public SqlGroupByCollection Add(params string[] columnBrackets)
        {
            columnBrackets.ForEach(columnBracket => base.Add(new SqlGroupBy(columnBracket)));
            return this;
        }

        public void BuildCommandText(StringBuilder commandText)
        {
            if (this.Count > 0)
            {
                commandText.Append("group by ", this.Select(o => o.ColumnBracket).Join(), " ");
            }
        }
    }
}
