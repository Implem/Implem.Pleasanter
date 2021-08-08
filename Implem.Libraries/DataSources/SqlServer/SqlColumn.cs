﻿using Implem.IRds;

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
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            string tableBracket,
            int? commandCount)
        {
            return Sub != null
                ? Sql_Sub(
                    factory: factory,
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandCount: commandCount)
                : CommandText(
                    factory: factory,
                    tableBracket: tableBracket)
                        + AsBracket();
        }

        private string AsBracket()
        {
            return As != null
                ? " as \"" + As + "\""
                : (Function != Sqls.Functions.None
                    ? " as \"" + ColumnName + Function.ToString() + "\""
                    : ColumnBracket.StartsWith("(")
                        ? " as \"" + ColumnName + "\""
                        : string.Empty);
        }

        private string CommandText(
            ISqlObjectFactory factory,
            string tableBracket)
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
                    return $"avg({factory.Sqls.IsNull}({columnBracket}, 0))";
                default:
                    return columnBracket;
            }
        }

        private string Sql_Sub(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            int? commandCount)
        {
            return "(" + Sub.GetCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                prefix: SubPrefix
                    ? "_sub"
                    : null,
                commandCount: commandCount) + ")" +
                    (As != null
                        ? " as \"" + As + "\""
                        : string.Empty);
        }
    }
}
