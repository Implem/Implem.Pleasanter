using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlColumnCollection : ListEx<SqlColumn>
    {
        public SqlColumnCollection Add(bool duplicates = false, params string[] columnBrackets)
        {
            columnBrackets.ForEach(columnBracket =>
            {
                if (duplicates || !this.Any(o => o.ColumnBracket == columnBracket))
                {
                    Add(new SqlColumn(columnBracket));
                }
            });
            return this;
        }

        public SqlColumnCollection Add(SqlStatement sub, string _as)
        {
            Add(new SqlColumn(sub, _as));
            return this;
        }

        public void BuildCommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount,
            bool distinct,
            int top)
        {
            commandText.Append("select ");
            Build_DistinctClause(commandText, distinct);
            Build_TopClause(commandText, top);
            commandText.Append(this
                .Select(o => o.CommandText(sqlContainer, sqlCommand, commandCount))
                .Join(), " ");
            RemoveAll(o => o.AdHoc);
        }

        private void Build_DistinctClause(StringBuilder commandText, bool distinct)
        {
            if (distinct)
            {
                commandText.Append("distinct ");
            }
        }

        private void Build_TopClause(StringBuilder commandText, int top)
        {
            if (top > 0)
            {
                commandText.Append("top ", top.ToString(), " ");
            }
        }
    }
}
