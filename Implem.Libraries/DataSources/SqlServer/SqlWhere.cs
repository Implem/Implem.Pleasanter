using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlWhere
    {
        public string[] ColumnBrackets;
        public string Name;
        public object Value;
        public string Operator;
        public string MultiColumnOperator;
        public string MultiParamOperator;
        public SqlStatement Sub;
        public string Raw;
        public SqlWhereCollection Or;
        public bool Using = true;

        public SqlWhere(
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
            ColumnBrackets = columnBrackets;
            Name = name;
            Value = value;
            Operator = _operator;
            MultiColumnOperator = multiColumnOperator;
            MultiParamOperator = multiParamOperator;
            Sub = sub;
            Raw = raw;
            Or = or;
            Using = _using;
        }

        public string Sql(SqlContainer sqlContainer, SqlCommand sqlCommand, int? commandCount)
        {
            if (Using)
            {
                if (!Raw.IsNullOrEmpty())
                {
                    return Sql_Raw(sqlContainer, commandCount);
                }
                else if (Sub != null)
                {
                    return Sql_Sub(sqlContainer, sqlCommand, commandCount);
                }
                else if (Or != null)
                {
                    return Sql_Or(sqlContainer, sqlCommand, commandCount);
                }
                else
                {
                    return Sql_General(commandCount);
                }
            }
            else
            {
                return string.Empty;
            }
        }

        private string Sql_General(int? commandCount)
        {
            if (Value.IsCollection())
            {
                var valueCollection = Value.ToStringEnumerable();
                if (valueCollection.Any())
                {
                    return "(" + ColumnBrackets
                         .Select(o => Sql_General(o, commandCount, valueCollection))
                         .Join(MultiColumnOperator) + ")";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return "(" + ColumnBrackets
                    .Select(o => Sql_General(o, commandCount))
                    .Join(MultiColumnOperator) + ")";
            }
        }

        private string Sql_General(
            string columnBracket,
            int? commandCount,
            IEnumerable<string> valueCollection)
        {
            return valueCollection
                .Select((o, i) => columnBracket + Operator + Variable(commandCount, i.ToString() + "_"))
                .Join(MultiParamOperator);
        }

        private string Sql_General(string columnBracket, int? commandCount)
        {
            return columnBracket + Operator + Variable(commandCount);
        }

        private string Variable(int? commandCount, string paramCount = "")
        {
            return Value != null && !Name.IsNullOrEmpty()
                ? "@" + Name + paramCount + commandCount.ToString()
                : string.Empty;
        }

        private string Sql_Sub(SqlContainer sqlContainer, SqlCommand sqlCommand, int? commandCount)
        {
            var commandText = Sub.GetCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                prefix: "_sub",
                commandCount: commandCount);
            return ColumnBrackets != null
                ? ColumnBrackets.Select(columnBracket =>
                    columnBracket + Operator + "(" + commandText + ")")
                        .Join(MultiColumnOperator)
                : Value == null
                    ? "(" + commandText + ")"
                    : "(" + commandText + Operator + Value + ")";
        }

        private string Sql_Raw(SqlContainer sqlContainer, int? commandCount)
        {
            if (Value.IsCollection())
            {
                var valueCollection = Value.ToStringEnumerable();
                return valueCollection
                    .Select((o, i) => ReplacedSql(commandCount, i.ToString()))
                    .Join(MultiParamOperator);
            }
            else
            {
                return ReplacedSql(sqlContainer, commandCount);
            }
        }

        private string ReplacedSql(SqlContainer sqlContainer, int? commandCount)
        {
            return ColumnBrackets != null
                ? ColumnBrackets.Select(columnBracket =>
                    columnBracket + ReplacedRaw(sqlContainer, commandCount))
                        .Join(MultiColumnOperator)
                : Raw.Replace("#CommandCount#", commandCount.ToString());
        }

        private string ReplacedRaw(SqlContainer sqlContainer, int? commandCount)
        {
            switch (Raw)
            {
                case "@@identity":
                    return Operator + "@_I";
                default:
                    return Operator + Raw.Replace("#CommandCount#", commandCount.ToString());
            }
        }

        private string ReplacedSql(int? commandCount, string paramCount)
        {
            return ColumnBrackets != null
                ? ColumnBrackets.Select(columnBracket =>
                    columnBracket + Operator + Raw
                        .Replace("#CommandCount#", commandCount.ToString())
                        .Replace("#ParamCount#", paramCount.ToString()))
                            .Join(MultiColumnOperator)
                : Raw
                    .Replace("#CommandCount#", commandCount.ToString())
                    .Replace("#ParamCount#", paramCount.ToString());
        }

        private string Sql_Or(SqlContainer sqlContainer, SqlCommand sqlCommand, int? commandCount)
        {
            var commandText = new StringBuilder();
            Or.Clause = string.Empty;
            Or.MultiClauseOperator = " or ";
            Or.BuildCommandText(sqlContainer, sqlCommand, commandText, commandCount);
            return "(" + commandText + ")";
        }
    }
}