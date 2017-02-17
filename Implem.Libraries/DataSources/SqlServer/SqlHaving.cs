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
            Using = _using;
        }

        public string Sql(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            Sqls.TableTypes tableType,
            int? commandCount)
        {
            if (Sub != null)
            {
                return Sql_Sub(sqlContainer, sqlCommand, tableType, commandCount);
            }
            else if (!Raw.IsNullOrEmpty())
            {
                return Sql_Raw(sqlContainer, tableType, commandCount);
            }
            else if (Or != null)
            {
                return Sql_Or(sqlContainer, sqlCommand, tableType, commandCount);
            }
            else
            {
                return Sql_General(tableType, commandCount);
            }
        }

        private string Sql_General(Sqls.TableTypes tableType, int? commandCount)
        {
            if (Value.IsCollection())
            {
                var valueCollection = Value.ToStringEnumerable();
                if (valueCollection.Any())
                {
                    return "(" + Functions(tableType) + ")";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return "(" + Functions(tableType) +
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
            Sqls.TableTypes tableType,
            int? commandCount)
        {
            var commandText = Sub.GetCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                prefix: "_sub",
                commandCount: commandCount);
            return ColumnBracket != null
                ? Functions(tableType) + Operator + "(" + commandText + ")"
                : Value == null
                    ? "(" + commandText + ")"
                    : "(" + commandText + Operator + Value + ")";
        }

        private string Sql_Raw(
            SqlContainer sqlContainer, Sqls.TableTypes tableType, int? commandCount)
        {
            if (Value.IsCollection())
            {
                var valueCollection = Value.ToStringEnumerable();
                return valueCollection
                    .Select((o, i) => ReplacedSql(tableType, commandCount, i.ToString()))
                    .Join(MultiParamOperator);
            }
            else
            {
                return ReplacedSql(sqlContainer, tableType, commandCount);
            }
        }

        private string ReplacedSql(
            SqlContainer sqlContainer, Sqls.TableTypes tableType, int? commandCount)
        {
            return ColumnBracket != null
                ? Functions(tableType) + ReplacedRaw(sqlContainer, commandCount)
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

        private string ReplacedSql(Sqls.TableTypes tableType, int? commandCount, string paramCount)
        {
            return ColumnBracket != null
                ? Functions(tableType) + Operator + Raw
                    .Replace("#CommandCount#", commandCount.ToString())
                    .Replace("#ParamCount#", paramCount.ToString())
                : Raw
                    .Replace("#CommandCount#", commandCount.ToString())
                    .Replace("#ParamCount#", paramCount.ToString());
        }

        private string Sql_Or(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            Sqls.TableTypes tableType,
            int? commandCount)
        {
            var commandText = new StringBuilder();
            Or.Clause = string.Empty;
            Or.MultiClauseOperator = " or ";
            Or.BuildCommandText(sqlContainer, sqlCommand, commandText, tableType, commandCount);
            return "(" + commandText + ")";
        }

        private string Functions(Sqls.TableTypes tableType)
        {
            return Function.ToString().ToLower() +
                "(" + Sqls.TableAndColumnBracket(
                    tableName: TableName,
                    tableType: tableType,
                    columnBracket: ColumnBracket) + ")";
        }
    }
}