using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlJoinCollection : ListEx<SqlJoin>
    {
        public SqlJoinCollection(params SqlJoin[] sqlJoinCollection)
        {
            this.AddRange(sqlJoinCollection);
        }

        public SqlJoinCollection Add(params string[] columnBrackets)
        {
            columnBrackets.ForEach(columnBracket => base.Add(new SqlJoin(columnBracket)));
            return this;
        }

        public void BuildCommandText(StringBuilder commandText)
        {
            commandText.Append(this.Select(o => o.JoinExpression).Join(" "));
        }
    }
}
