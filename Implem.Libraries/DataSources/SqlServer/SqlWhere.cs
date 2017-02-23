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
        public string TableName;
        public string Name;
        public object Value;
        public string Operator;
        public string MultiColumnOperator;
        public string MultiParamOperator;
        public SqlStatement SubLeft;
        public SqlStatement Sub;
        public string Raw;
        public SqlWhereCollection Or;
        public bool Using = true;

        public SqlWhere(
            string[] columnBrackets = null,
            string tableName = null,
            string name = null,
            object value = null,
            string _operator = "=",
            string multiColumnOperator = " or ",
            string multiParamOperator = " and ",
            SqlStatement subLeft = null,
            SqlStatement sub = null,
            string raw = null,
            SqlWhereCollection or = null,
            bool _using = true)
        {
            ColumnBrackets = columnBrackets;
            TableName = tableName;
            Name = name;
            Value = value;
            Operator = _operator;
            MultiColumnOperator = multiColumnOperator;
            MultiParamOperator = multiParamOperator;
            SubLeft = subLeft;
            Sub = sub;
            Raw = raw;
            Or = or;
            Using = _using;
            Or?.Where(o => o != null).ForEach(o => o.Name += "_or");
        }

        public string Sql(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            Sqls.TableTypes tableType,
            int? commandCount)
        {
            if (Using)
            {
                var left = Left(
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandCount: commandCount);
                if (!Raw.IsNullOrEmpty())
                {
                    return Sql_Raw(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        left: left,
                        tableType: tableType,
                        commandCount: commandCount);
                }
                else if (Sub != null)
                {
                    return Sql_Sub(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        left: left,
                        tableType: tableType,
                        commandCount: commandCount);
                }
                else if (Or != null)
                {
                    return Sql_Or(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        tableType: tableType,
                        commandCount: commandCount);
                }
                else
                {
                    return Sql_General(
                        left: left,
                        tableType: tableType,
                        commandCount: commandCount);
                }
            }
            else
            {
                return string.Empty;
            }
        }

        private string Sql_General(
            IEnumerable<string> left, Sqls.TableTypes tableType, int? commandCount)
        {
            if (Value.IsCollection())
            {
                var valueCollection = Value.ToStringEnumerable();
                if (valueCollection.Any())
                {
                    return "(" + left
                         .Select(columnBracket =>
                            Sql_General(
                                columnBracket: columnBracket,
                                tableType: tableType,
                                commandCount: commandCount,
                                valueCollection: valueCollection))
                         .Join(MultiColumnOperator) + ")";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return "(" + left
                    .Select(columnBracket =>
                        Sql_General(
                            columnBracket: columnBracket,
                            tableType: tableType,
                            commandCount: commandCount))
                    .Join(MultiColumnOperator) + ")";
            }
        }

        private string Sql_General(
            string columnBracket,
            Sqls.TableTypes tableType,
            int? commandCount,
            IEnumerable<string> valueCollection)
        {
            return valueCollection
                .Select((o, i) =>
                    Sqls.TableAndColumnBracket(
                        tableName: TableName,
                        tableType: tableType,
                        columnBracket: columnBracket) +
                    Operator +
                    Variable(
                        commandCount: commandCount,
                        paramCount: i.ToString() + "_"))
                .Join(MultiParamOperator);
        }

        private string Sql_General(
            string columnBracket, Sqls.TableTypes tableType, int? commandCount)
        {
            return
                Sqls.TableAndColumnBracket(
                    tableName: TableName,
                    tableType: tableType,
                    columnBracket: columnBracket) +
                Operator +
                Variable(commandCount: commandCount);
        }

        private string Variable(int? commandCount, string paramCount = "")
        {
            return Value != null && !Name.IsNullOrEmpty()
                ? "@" + Name + paramCount + commandCount.ToString()
                : string.Empty;
        }

        private string Sql_Sub(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            IEnumerable<string> left,
            Sqls.TableTypes tableType,
            int? commandCount)
        {
            var commandText = Sub.GetCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                prefix: "_sub",
                commandCount: commandCount);
            return left != null
                ? left.Select(columnBracket =>
                    Sqls.TableAndColumnBracket(
                        tableName: TableName,
                        tableType: tableType,
                        columnBracket: columnBracket) +
                    Operator +
                    "(" + commandText + ")")
                        .Join(MultiColumnOperator)
                : Value == null
                    ? "(" + commandText + ")"
                    : "(" + commandText + Operator + Value + ")";
        }

        private string Sql_Raw(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            IEnumerable<string> left,
            Sqls.TableTypes tableType,
            int? commandCount)
        {
            if (Value.IsCollection())
            {
                return Value.ToStringEnumerable()
                    .Select((o, i) => ReplacedSql(
                        left: left,
                        commandCount: commandCount,
                        paramCount: i.ToString()))
                    .Join(MultiParamOperator);
            }
            else
            {
                return ReplacedSql(
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    left: left,
                    tableType: tableType,
                    commandCount: commandCount);
            }
        }

        private string ReplacedSql(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            IEnumerable<string> left,
            Sqls.TableTypes tableType,
            int? commandCount)
        {
            return left != null
                ? left.Select(columnBracket =>
                    Sqls.TableAndColumnBracket(
                        tableName: TableName,
                        tableType: tableType,
                        columnBracket: columnBracket) +
                    ReplacedRaw(commandCount: commandCount))
                        .Join(MultiColumnOperator)
                : Raw.Replace("#CommandCount#", commandCount.ToString());
        }

        private string ReplacedRaw(int? commandCount)
        {
            switch (Raw)
            {
                case "@@identity":
                    return Operator + "@_I";
                default:
                    return Operator + Raw.Replace("#CommandCount#", commandCount.ToString());
            }
        }

        private string ReplacedSql(IEnumerable<string> left, int? commandCount, string paramCount)
        {
            var raw = Raw
                .Replace("#CommandCount#", commandCount.ToString())
                .Replace("#ParamCount#", paramCount.ToString());
            return left != null
                ? left.Select(columnBracket =>
                    columnBracket + Operator + "(" + raw + ")")
                        .Join(MultiColumnOperator)
                : raw;
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
            Or.BuildCommandText(
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                tableType: tableType,
                commandCount: commandCount);
            return "(" + commandText + ")";
        }

        private IEnumerable<string> Left(
            SqlContainer sqlContainer,
            SqlCommand sqlCommand,
            int? commandCount)
        {
            if (SubLeft != null)
            {
                return new List<string>
                {
                    "(" + SubLeft.GetCommandText(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        prefix: "_sub",
                        commandCount: commandCount) + ")"
                };
            }
            else
            {
                return ColumnBrackets;
            }
        }
    }
}