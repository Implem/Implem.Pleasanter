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
            return $@"cast(""{left}"".""{name}"" as {type})";
        }

        public string CreateUpdateOrInsert(
            string tableBracket,
            string setClause,
            Action<StringBuilder> sqlWhereAppender,
            string intoClause,
            string valueClause,
            string selectClauseForMySql)
        {
            //MySQLには一文でUpsertを実行可能な構文がないため、
            //同一のwhere条件文を使ったupdate～;とinsert～;の2件を組み合わせて実現する。
            var update = new StringBuilder($@"
                update {tableBracket} {setClause}");
            sqlWhereAppender(update);
            update.Append("; ");
            var insert = new StringBuilder($@"
                insert into {tableBracket} ({intoClause})
                select * from (select {selectClauseForMySql} ) as tmp
                where not exists (select 1 from {tableBracket}");
            sqlWhereAppender(insert);
            insert.Append(")");
            return String.Concat(update.ToString(), insert.ToString());
        }

        public string CreateFullTextWhereItem(
            string itemsTableName,
            string paramName,
            bool negative)
        {
            return (negative
                ? $@"not match(""{itemsTableName}"".""FullText"") against (@{paramName}#CommandCount# in boolean mode)"
                : $@"match(""{itemsTableName}"".""FullText"") against (@{paramName}#CommandCount# in boolean mode)");
        }

        public string CreateFullTextWhereBinary(
            string itemsTableName,
            string paramName,
            bool negative)
        {
            //MySQL版はBinaries.BinにFullTextIndexを付けることができないので、バイナリ検索機能は提供不可。
            return "0=1";
        }

        public Dictionary<string, string> CreateSearchTextWords(
            Dictionary<string, string> words,
            string searchText)
        {
            return words;
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
