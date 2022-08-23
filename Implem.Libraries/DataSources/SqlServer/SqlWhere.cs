﻿using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Implem.Libraries.DataSources.SqlServer
{
    public class SqlWhere
    {
        public string TableName;
        public string[] JoinTableNames;
        public string[] ColumnBrackets;
        public string Name;
        public object Value;
        public string Operator;
        public string MultiColumnOperator;
        public string MultiParamOperator;
        public SqlStatement SubLeft;
        public SqlStatement Sub;
        public bool SubPrefix;
        public string Raw;
        public SqlWhereCollection And;
        public SqlWhereCollection Or;
        public bool Using = true;

        public SqlWhere(
            string tableName = null,
            string[] joinTableNames = null,
            string[] columnBrackets = null,
            string name = null,
            object value = null,
            string _operator = "=",
            string multiColumnOperator = " or ",
            string multiParamOperator = " and ",
            SqlStatement subLeft = null,
            SqlStatement sub = null,
            bool subPrefix = true,
            string raw = null,
            SqlWhereCollection and = null,
            SqlWhereCollection or = null,
            bool _using = true)
        {
            TableName = tableName;
            JoinTableNames = joinTableNames;
            ColumnBrackets = columnBrackets;
            Name = name;
            Value = value;
            Operator = _operator;
            MultiColumnOperator = multiColumnOperator;
            MultiParamOperator = multiParamOperator;
            SubLeft = subLeft;
            Sub = sub;
            SubPrefix = subPrefix;
            Raw = raw;
            And = and;
            Or = or;
            Using = _using;
        }

        public string Sql(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            string tableBracket,
            int? commandCount,
            bool select)
        {
            if (Using)
            {
                var left = Left(
                    factory: factory,
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    commandCount: commandCount);
                if (!Raw.IsNullOrEmpty())
                {
                    return Sql_Raw(
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        left: left,
                        tableBracket: tableBracket,
                        commandCount: commandCount,
                        select: select);
                }
                else if (Sub != null)
                {
                    return Sql_Sub(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        left: left,
                        tableBracket: tableBracket,
                        commandCount: commandCount,
                        select: select);
                }
                else if (And != null)
                {
                    return Sql_And(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        commandCount: commandCount,
                        select: select);
                }
                else if (Or != null)
                {
                    return Sql_Or(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        tableBracket: tableBracket,
                        commandCount: commandCount,
                        select: select);
                }
                else
                {
                    return Sql_General(
                        left: left,
                        tableBracket: tableBracket,
                        commandCount: commandCount,
                        select: select);
                }
            }
            else
            {
                return string.Empty;
            }
        }

        private string Sql_General(
            IEnumerable<string> left,
            string tableBracket,
            int? commandCount,
            bool select)
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
                                tableBracket: tableBracket,
                                commandCount: commandCount,
                                valueCollection: valueCollection,
                                select: select))
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
                            tableBracket: tableBracket,
                            commandCount: commandCount,
                            select: select))
                    .Join(MultiColumnOperator) + ")";
            }
        }

        private string Sql_General(
            string columnBracket,
            string tableBracket,
            int? commandCount,
            IEnumerable<string> valueCollection,
            bool select)
        {
            return valueCollection
                .Select((o, i) =>
                    TableAndColumnBracket(
                        tableBracket: tableBracket,
                        columnBracket: columnBracket,
                        select: select)
                        + OperatorAndVariable(
                            commandCount: commandCount,
                            paramCount: i.ToString() + "_"))
                .Join(MultiParamOperator);
        }

        private string Sql_General(
            string columnBracket,
            string tableBracket,
            int? commandCount,
            bool select)
        {
            return
                TableAndColumnBracket(
                    tableBracket: tableBracket,
                    columnBracket: columnBracket,
                    select: select)
                    + OperatorAndVariable(commandCount: commandCount);
        }

        private string OperatorAndVariable(
            int? commandCount,
            string paramCount = "")
        {
            if (Value != null && !Name.IsNullOrEmpty())
            {
                var right = "@" + Name + paramCount + commandCount.ToString();
                if (Operator?.Contains("{0}") == true)
                {
                    return Operator.Params(right);
                }
                else
                {
                    return Operator + right;
                }
            }
            else
            {
                return Operator;
            }
        }

        private string Sql_Sub(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            IEnumerable<string> left,
            string tableBracket,
            int? commandCount,
            bool select)
        {
            var commandText = Sub.GetCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                prefix: SubPrefix
                    ? "_sub"
                    : null,
                commandCount: commandCount);
            return left != null
                ? left.Select(columnBracket =>
                    TableAndColumnBracket(
                        tableBracket: tableBracket,
                        columnBracket: columnBracket,
                        select: select) +
                    Operator +
                    "(" + commandText + ")")
                        .Join(MultiColumnOperator)
                : Value == null
                    ? "(" + commandText + ")"
                    : "(" + commandText + Operator + Value + ")";
        }

        private string Sql_Raw(
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            IEnumerable<string> left,
            string tableBracket,
            int? commandCount,
            bool select)
        {
            if (Value.IsCollection())
            {
                return Value.ToStringEnumerable()
                    .Select((o, i) => ReplacedRaw(
                        left: left,
                        tableBracket: tableBracket,
                        commandCount: commandCount,
                        paramCount: i.ToString()))
                    .Join(MultiParamOperator);
            }
            else
            {
                return ReplacedRaw(
                    sqlContainer: sqlContainer,
                    sqlCommand: sqlCommand,
                    left: left,
                    tableBracket: tableBracket,
                    commandCount: commandCount,
                    select: select);
            }
        }

        private string ReplacedRaw(
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            IEnumerable<string> left,
            string tableBracket,
            int? commandCount,
            bool select)
        {
            return left != null
                ? left.Select(columnBracket =>
                    TableAndColumnBracket(
                        tableBracket: tableBracket,
                        columnBracket: columnBracket,
                        select: select) +
                    ReplacedRaw(
                        commandCount: commandCount,
                        tableBracket: tableBracket))
                            .Join(MultiColumnOperator)
                : Raw
                    .Replace("#TableBracket#", tableBracket)
                    .Replace("#CommandCount#", commandCount.ToString());
        }

        private string ReplacedRaw(int? commandCount, string tableBracket)
        {
            switch (Raw)
            {
                case "@@identity":
                    return Operator + $"{Parameters.Parameter.SqlParameterPrefix}I";
                default:
                    return Operator + Raw
                        .Replace("#TableBracket#", tableBracket)
                        .Replace("#CommandCount#", commandCount.ToString());
            }
        }

        private string ReplacedRaw(
            IEnumerable<string> left,
            string tableBracket,
            int? commandCount,
            string paramCount)
        {
            var raw = Raw
                .Replace("#TableBracket#", tableBracket)
                .Replace("#CommandCount#", commandCount.ToString())
                .Replace("#ParamCount#", paramCount.ToString());
            return left != null
                ? left.Select(columnBracket =>
                    columnBracket + Operator + "(" + raw + ")")
                        .Join(MultiColumnOperator)
                : raw;
        }

        private string Sql_And(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            int? commandCount,
            bool select)
        {
            var commandText = new StringBuilder();
            And.Clause = string.Empty;
            And.BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount,
                multiClauseOperator: " and ",
                select: select);
            return "(" + commandText + ")";
        }

        private string Sql_Or(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            string tableBracket,
            int? commandCount,
            bool select)
        {
            var commandText = new StringBuilder();
            Or.Clause = string.Empty;
            Or.MultiClauseOperator = " or ";
            Or.BuildCommandText(
                factory: factory,
                sqlContainer: sqlContainer,
                sqlCommand: sqlCommand,
                commandText: commandText,
                commandCount: commandCount,
                multiClauseOperator: " or ",
                select: select);
            return "(" + commandText + ")";
        }

        private IEnumerable<string> Left(
            ISqlObjectFactory factory,
            SqlContainer sqlContainer,
            ISqlCommand sqlCommand,
            int? commandCount)
        {
            if (SubLeft != null)
            {
                return new List<string>
                {
                    "(" + SubLeft.GetCommandText(
                        factory: factory,
                        sqlContainer: sqlContainer,
                        sqlCommand: sqlCommand,
                        prefix: SubPrefix
                            ? "_sub"
                            : null,
                        commandCount: commandCount) + ")"
                };
            }
            else
            {
                return ColumnBrackets;
            }
        }

        private string TableAndColumnBracket(
            string tableBracket, string columnBracket, bool select)
        {
            return select
                ? Sqls.TableAndColumnBracket(tableBracket, columnBracket)
                : columnBracket;
        }
    }
}