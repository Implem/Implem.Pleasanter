using System.Data.SqlClient;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlColumn
    {
        public string ColumnBracket;
        public string TableName;
        public string ColumnName;
        public string As;
        public Sqls.Functions Function;
        public bool AdHoc;
        public SqlStatement Sub;
        bool SubPrefix;

        public SqlColumn()
        {
        }

        public SqlColumn(
            string columnBracket,
            string tableName = null,
            string columnName = null,
            string _as = null,
            Sqls.Functions function = Sqls.Functions.None,
            bool adHoc = false,
            SqlStatement sub = null,
            bool subPrefix = true)
        {
            ColumnBracket = columnBracket;
            TableName = tableName;
            ColumnName = columnName;
            As = _as;
            Function = function;
            AdHoc = adHoc;
            Sub = sub;
            SubPrefix = subPrefix;
        }

        public string CommandText(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            string tableBracket,
            int? commandCount)
        {
            return Sub != null
                ? Sql_Sub(
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandCount: commandCount)
                : CommandText(tableBracket: tableBracket) + AsBracket();
        }

        private string AsBracket()
        {
            return As != null
                ? " as [" + As + "]"
                : (Function != Sqls.Functions.None
                    ? " as [" + ColumnName + Function.ToString() + "]"
                    : ColumnBracket.StartsWith("(")
                        ? " as [" + ColumnName + "]"
                        : string.Empty);
        }

        private string CommandText(string tableBracket)
        {
            var columnBracket = Sqls.TableAndColumnBracket(
                tableBracket: tableBracket, columnBracket: ColumnBracket);
            switch (Function)
            {
                case Sqls.Functions.Count:
                    return "count(*)";
                case Sqls.Functions.Sum:
                    return "sum(" + columnBracket + ")";
                case Sqls.Functions.Min:
                    return "min(" + columnBracket + ")";
                case Sqls.Functions.Max:
                    return "max(" + columnBracket + ")";
                case Sqls.Functions.Avg:
                    return "avg(" + columnBracket + ")";
                default:
                    return columnBracket;
            }
        }

        private string Sql_Sub(SqlContainer sqlContainer, SqlCommand sqlCommand, int? commandCount)
        {
            return "(" + Sub.GetCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                prefix: SubPrefix
                    ? "_sub"
                    : null,
                commandCount: commandCount) + ")" +
                    (As != null
                        ? " as [" + As + "]"
                        : string.Empty);
        }
    }
}
