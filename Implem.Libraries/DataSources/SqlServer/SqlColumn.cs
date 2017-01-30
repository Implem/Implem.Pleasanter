using System.Data.SqlClient;

namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlColumn
    {
        public string ColumnBracket;
        public SqlStatement Sub;
        public string As;
        public bool AdHoc;

        public SqlColumn()
        {
        }

        public SqlColumn(string columnBracket, bool adHoc = false)
        {
            ColumnBracket = columnBracket;
            AdHoc = adHoc;
        }

        public SqlColumn(SqlStatement sub, string _as)
        {
            Sub = sub;
            As = _as;
        }

        public string CommandText(
            SqlContainer sqlContainer, SqlCommand sqlCommand, int? commandCount)
        {
            return Sub != null
                ? Sql_Sub(sqlContainer, sqlCommand, commandCount)
                : ColumnBracket;
        }

        private string Sql_Sub(SqlContainer sqlContainer, SqlCommand sqlCommand, int? commandCount)
        {
            return "(" + Sub.GetCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                prefix: "_sub",
                commandCount: commandCount) + ")" +
                    (As != null
                        ? " as [" + As + "]"
                        : string.Empty);
        }
    }
}
