using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
namespace Implem.MySql
{
    internal class MySqlCommandText : ISqlCommandText
    {
        public string BeforeAllCommand()
        {
            return "set session sql_mode = 'ansi_quotes,pipes_as_concat';";
        }

        public string CreateDelete(string template)
        {
            return template + " ; select row_count() ";
        }

        public string CreateRestore(string template)
        {
            return template + " ; select row_count() ";
        }

        public string CreateIdentityInsert(string template)
        {
            return string.Empty;
        }

        public string CreateLimitClause(int limit)
        {
            return limit > 0 ? $" limit {limit} " : string.Empty;
        }

        public string CreateSelectIdentity(
            string dataTableName,
            string template,
            string identityColumnName)
        {
            return string.Format(
                template,
                dataTableName ?? string.Empty);
        }

        public string CreateSelectStatementTerminator(bool selectIdentity)
        {
            return ";";
        }

        public string CreateTopClause(int top)
        {
            return string.Empty;
        }

        public string CreateTryCast(string left, string name, string from, string to)
        {
            //今後toがとる値のバリエーションが増えた場合、case文の追加が必要。
            string type;
            switch (to)
            {
                case "bigint":
                    type = "signed";
                    break;
                default:
                    type = to;
                    break;
            }
            return $"cast(\"{left}\".\"{name}\" as {type})";
        }

        public string CreateUpdateOrInsert(
            string tableBracket,
            string setClause,
            Action<StringBuilder> sqlWhereAppender,
            string intoClause,
            string valueClause,
            string selectClauseForMySql)
        {
            var commandText = new StringBuilder();
            //MySQLには一文でUpsertを実行可能な構文がないため、同一のwhere条件を含むupdate～;とinsert～;の2件を組み合わせて実現する。
            commandText
                .Append("update ")
                .Append(tableBracket)
                .Append(setClause);
            sqlWhereAppender(commandText);
            commandText
                .Append("; ")
                .Append("insert into ")
                .Append(tableBracket)
                .Append(" ( ")
                .Append(intoClause)
                .Append(" ) ")
                .Append("select * from ")
                .Append("( ")
                .Append("select ")
                .Append(selectClauseForMySql)
                .Append(" ) as tmp ")
                .Append("where not exists ")
                .Append("( select 1 from ")
                .Append(tableBracket)
                .Append(" ");
            sqlWhereAppender(commandText);
            commandText
                .Append(" )");
            return commandText.ToString();
        }

        public string CreateFullTextWhereItem(
            string itemsTableName,
            string paramName,
            bool negative)
        {
            return (negative
                ? $"not match(\"{itemsTableName}\".\"FullText\") against (@{paramName}#CommandCount#)"
                : $"match(\"{itemsTableName}\".\"FullText\") against (@{paramName}#CommandCount#)");
        }

        public string CreateFullTextWhereBinary(
            string itemsTableName,
            string paramName,
            bool negative)
        {
            //MySQL版はBinaries.BinにFullTextIndexを付けていないので、バイナリ検索機能は提供不可。
            return "0=1";
        }

        public Dictionary<string, string> CreateSearchTextWords(
            Dictionary<string, string> words,
            string searchText)
        {
            return new Dictionary<string, string> { [Strings.NewGuid()] = searchText };
        }

        public string CreateDataRangeCommand(int? commandCount)
        {
            return $"limit {Parameters.Parameter.SqlParameterPrefix}PageSize" +
                commandCount.ToString() +
                $" offset {Parameters.Parameter.SqlParameterPrefix}Offset" +
                commandCount.ToString();
        }
    }
}
