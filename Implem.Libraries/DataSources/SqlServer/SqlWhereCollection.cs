using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlWhereCollection : ListEx<SqlWhere>
    {
        public string Clause = "where ";
        public string MultiClauseOperator = " and ";

        public SqlWhereCollection(params SqlWhere[] sqlWhereCollection)
        {
            AddRange(sqlWhereCollection);
        }

        public SqlWhereCollection Add(
            string[] columnBrackets = null,
            string name = "",
            object value = null,
            string _operator = "=",
            string multiColumnOperator = " or ",
            string multiParamOperator = " and ",
            SqlStatement sub = null,
            string raw = "",
            SqlWhereCollection or = null,
            bool _using = true)
        {
            Add(new SqlWhere(
                columnBrackets: columnBrackets,
                name: name,
                value: value,
                _operator: _operator,
                multiColumnOperator: multiColumnOperator,
                multiParamOperator: multiParamOperator,
                sub: sub,
                raw: raw,
                or: or,
                _using: _using));
            return this;
        }

        public void BuildCommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            StringBuilder commandText,
            int? commandCount,
            bool select = false)
        {
            commandText.Append(Sql(sqlContainer, sqlCommand, commandCount, select));
        }

        public string Sql(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            int? commandCount,
            bool select = false)
        {
            if (!select)
            {
                this.Where(o => o?.ColumnBrackets != null)
                    .ForEach(where => where.ColumnBrackets =
                        where.ColumnBrackets.Select(o => o.Split('.').Last()).ToArray());
            }
            return this.Where(o => o != null).Any(o => o.Using)
                ? Clause + this
                    .Where(o => o != null)
                    .Where(o => o.Using)
                    .Select(o => o.Sql(sqlContainer, sqlCommand, commandCount))
                    .Join(MultiClauseOperator) + " "
                : string.Empty;
        }

        public void Prefix(string prefix)
        {
            ForEach(o => o.Name += prefix);
        }
    }
}