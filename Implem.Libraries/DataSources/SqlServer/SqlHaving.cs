using Implem.Libraries.Utilities;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlHaving
    {
        public string ColumnBracket;
        public string TableName;
        public object Value;
        public string Operator;
        public string MultiParamOperator;
        public SqlStatement Sub;
        public string Raw;
        public SqlHavingCollection Or;
        public Sqls.Functions Function;
        public bool SubPrefix;
        public bool Using = true;

        public SqlHaving(
            string columnBracket,
            string tableName = null,
            object value = null,
            string _operator = "=",
            string multiParamOperator = " and ",
            SqlStatement sub = null,
            string raw = null,
            SqlHavingCollection or = null,
            Sqls.Functions function = Sqls.Functions.None,
            bool subPrefix = true,
            bool _using = true)
        {
            ColumnBracket = columnBracket;
            TableName = tableName;
            Value = value;
            Operator = _operator;
            MultiParamOperator = multiParamOperator;
            Sub = sub;
            Raw = raw;
            Or = or;
            Function = function;
            SubPrefix = subPrefix;
            Using = _using;
        }

        public string Sql(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            string tableBracket,
            int? commandCount)
        {
            if (Sub != null)
            {
                return Sql_Sub(sqlContainer, sqlCommand, tableBracket, commandCount);
            }
            else if (!Raw.IsNullOrEmpty())
            {
                return Sql_Raw(sqlContainer, tableBracket, commandCount);
            }
            else if (Or != null)
            {
                return Sql_Or(sqlContainer, sqlCommand, commandCount);
            }
            else
            {
                return Sql_General(tableBracket, commandCount);
            }
        }

        private string Sql_General(string tableBracket, int? commandCount)
        {
            if (Value.IsCollection())
            {
                var valueCollection = Value.ToStringEnumerable();
                if (valueCollection.Any())
                {
                    return "(" + Functions(tableBracket) + ")";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return "(" + Functions(tableBracket) +
                    Operator +
                    Variable(commandCount) + ")";
            }
        }

        private string Variable(int? commandCount, string paramCount = "")
        {
            return Value != null && !TableName.IsNullOrEmpty()
                ? "@" + TableName + paramCount + commandCount.ToString()
                : string.Empty;
        }

        private string Sql_Sub(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            string tableBracket,
            int? commandCount)
        {
            var commandText = Sub.GetCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                prefix: SubPrefix
                    ? "_sub"
                    : string.Empty,
                commandCount: commandCount);
            return ColumnBracket != null
                ? Functions(tableBracket) + Operator + "(" + commandText + ")"
                : Value == null
                    ? "(" + commandText + ")"
                    : "(" + commandText + Operator + Value + ")";
        }

        private string Sql_Raw(
            SqlContainer sqlContainer,
            string tableBracket,
            int? commandCount)
        {
            if (Value.IsCollection())
            {
                var valueCollection = Value.ToStringEnumerable();
                return valueCollection
                    .Select((o, i) => ReplacedSql(tableBracket, commandCount, i.ToString()))
                    .Join(MultiParamOperator);
            }
            else
            {
                return ReplacedSql(sqlContainer, tableBracket, commandCount);
            }
        }

        private string ReplacedSql(
            SqlContainer sqlContainer, string tableBracket, int? commandCount)
        {
            return ColumnBracket != null
                ? Functions(tableBracket) + ReplacedRaw(sqlContainer, commandCount)
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

        private string ReplacedSql(string tableBracket, int? commandCount, string paramCount)
        {
            return ColumnBracket != null
                ? Functions(tableBracket) + Operator + Raw
                    .Replace("#CommandCount#", commandCount.ToString())
                    .Replace("#ParamCount#", paramCount.ToString())
                : Raw
                    .Replace("#CommandCount#", commandCount.ToString())
                    .Replace("#ParamCount#", paramCount.ToString());
        }

        private string Sql_Or(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            int? commandCount)
        {
            var commandText = new StringBuilder();
            Or.Clause = string.Empty;
            Or.MultiClauseOperator = " or ";
            Or.BuildCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount);
            return "(" + commandText + ")";
        }

        private string Functions(string tableBracket)
        {
            return Function.ToString().ToLower() +
                "(" + Sqls.TableAndColumnBracket(
                    tableBracket: tableBracket,
                    columnBracket: ColumnBracket) + ")";
        }
    }
}