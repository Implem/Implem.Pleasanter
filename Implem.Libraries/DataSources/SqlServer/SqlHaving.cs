using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlHaving
    {
        public string ColumnBracket;
        public string Name;
        public object Value;
        public string Operator;
        public string MultiParamOperator;
        public SqlStatement Sub;
        public string Raw;
        public SqlHavingCollection Or;
        public bool Using = true;

        public SqlHaving(
            string columnBracket,
            string name = "",
            object value = null,
            string _operator = "=",
            string multiParamOperator = " and ",
            SqlStatement sub = null,
            string raw = "",
            SqlHavingCollection or = null,
            bool _using = true)
        {
            ColumnBracket = columnBracket;
            Name = name;
            Value = value;
            Operator = _operator;
            MultiParamOperator = multiParamOperator;
            Sub = sub;
            Raw = raw;
            Or = or;
            Using = _using;
        }

        public string Sql(SqlContainer sqlContainer, SqlCommand sqlCommand, int? commandCount)
        {
            if (Sub != null)
            {
                return Sql_Sub(sqlContainer, sqlCommand, commandCount);
            }
            else if (!Raw.IsNullOrEmpty())
            {
                return Sql_Raw(sqlContainer, commandCount);
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

        private string Sql_General(int? commandCount)
        {
            if (Value.IsCollection())
            {
                var valueCollection = Value.ToStringEnumerable();
                if (valueCollection.Any())
                {
                    return "(" + ColumnBracket + ")";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return "(" + ColumnBracket + Operator + Variable(commandCount) +  ")";
            }
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
            return ColumnBracket != null
                ? ColumnBracket + Operator + "(" + commandText + ")"
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
            return ColumnBracket != null
                ? ColumnBracket + ReplacedRaw(sqlContainer, commandCount)
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
            return ColumnBracket != null
                ? ColumnBracket + Operator + Raw
                    .Replace("#CommandCount#", commandCount.ToString())
                    .Replace("#ParamCount#", paramCount.ToString())
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