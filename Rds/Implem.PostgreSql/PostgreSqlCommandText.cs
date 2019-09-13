using Implem.IRds;
using System;
using System.Collections.Generic;
using System.Text;
namespace Implem.PostgreSql
{
    internal class PostgreSqlCommandText : ISqlCommandText
    {
        public string CreateDelete(string template)
        {
            return template + " RETURNING * ";
        }

        public string CreateRestore(string template)
        {
            return template + " RETURNING * ";
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
            string template,
            string identityColumnName)
        {
            return string.Format(template, identityColumnName);
        }

        public string CreateSelectStatementTerminator(bool selectIdentity)
        {
            return selectIdentity ? "" : ";";
        }

        public string CreateTopClause(int top)
        {
            return string.Empty;
        }

        public string CreateTryCast(string left, string name, string type)
        {
            return $"(CASE WHEN \"{left}\".\"{name}\"~E'^\\d+$' THEN \"{left}\".\"{name}\"::{type} ELSE null END)";
        }

        public string CreateUpdateOrInsert(
            string tableBracket,
            string setClause,
            Action<StringBuilder> sqlWhereAppender,
            string intoClause,
            string valueClause)
        {
            var commandText = new StringBuilder();

            commandText
                .Append(" with CTE1 as ( ")
                .Append("update ")
                .Append(tableBracket)
                .Append(setClause);
            sqlWhereAppender(commandText);
            commandText
                .Append(" returning 0 ")
                .Append(" ) ")
                .Append(" insert into ")
                .Append(tableBracket)
                .Append(" ( ")
                .Append(intoClause)
                .Append(" ) select ")
                .Append(valueClause)
                .Append(" where not exists(select * from CTE1) ");
            return commandText.ToString();
        }

        public string CreateFullTextWhereItem(string itemsTableName, int count)
        {
            return $"(\"{itemsTableName}\".\"FullText\" %> @SearchText{count}_#CommandCount#)";
        }

        public string CreateFullTextWhereBinary(
            string itemsTableName,
            int count)
        {
            return $"(exists(select * from \"Binaries\" where \"Binaries\".\"ReferenceId\"=\"{itemsTableName}\".\"ReferenceId\" and (encode(\"Bin\", 'escape') %> @SearchText{count}_#CommandCount#)))";
        }

        public List<string> CreateSearchTextWords(
            List<string> words,
            string searchText)
        {
            return new List<string> { searchText };
        }
    }
}
