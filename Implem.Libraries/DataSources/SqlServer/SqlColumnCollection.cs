using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlColumnCollection : ListEx<SqlColumn>
    {
        public SqlColumnCollection Add(
            string columnBracket = null,
            string tableName = null,
            string columnName = null,
            string _as = null,
            Sqls.Functions function = Sqls.Functions.None,
            SqlStatement sub = null)
        {
            if (!this.Any(o =>
                o.ColumnBracket == columnBracket &&
                o.TableName == tableName &&
                o.ColumnName == columnName &&
                o.As == _as &&
                o.Function == function &&
                o.Sub == sub))
            {
                Add(new SqlColumn(
                    columnBracket: columnBracket,
                    tableName: tableName,
                    columnName: columnName,
                    _as: _as,
                    function: function,
                    sub: sub));
            }
            return this;
        }

        public void BuildCommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            Sqls.TableTypes tableType,
            int? commandCount,
            bool distinct,
            int top)
        {
            commandText.Append("select ");
            Build_DistinctClause(commandText, distinct);
            Build_TopClause(commandText, top);
            commandText.Append(this
                .Select(o => o.CommandText(sqlContainer, sqlCommand, tableType, commandCount))
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
