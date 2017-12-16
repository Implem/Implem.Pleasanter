using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlColumnCollection : ListEx<SqlColumn>
    {
        public SqlColumnCollection(params SqlColumn[] sqlColumnCollection)
        {
            sqlColumnCollection.ForEach(sqlColumn => Add(sqlColumn));
        }

        public SqlColumnCollection Add(
            string columnBracket = null,
            string tableName = null,
            string columnName = null,
            string _as = null,
            Sqls.Functions function = Sqls.Functions.None,
            SqlStatement sub = null,
            bool subPrefix = true)
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
                    sub: sub,
                    subPrefix: subPrefix));
            }
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
            if (this.Any())
            {
                commandText.Append(this
                    .Select(o => o.CommandText(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        tableBracket: Sqls.GetTableBracket(o.TableName),
                        commandCount: commandCount))
                    .Join(), " ");
            }
            else
            {
                commandText.Append("* ");
            }
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
