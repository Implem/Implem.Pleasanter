using Implem.IRds;
using System;
using System.Collections.Generic;
using System.Text;
namespace Implem.SqlServer
{
    internal class SqlServerCommandText : ISqlCommandText
    {
        public string CreateDelete(string template)
        {
            return template + " ; select @@rowcount ";
        }

        public string CreateRestore(string template)
        {
            return template + " ; select @@rowcount ";
        }

        public string CreateIdentityInsert(string template)
        {
            return template;
        }

        public string CreateLimitClause(int limit)
        {
            return string.Empty;
        }

        public string CreateSelectIdentity(
            string template,
            string identityColumnName)
        {
            return template;
        }

        public string CreateSelectStatementTerminator(bool selectIdentity)
        {
            return ";";
        }

        public string CreateTopClause(int top)
        {
            return top > 0 ? $" top {top} " : string.Empty;
        }

        public string CreateTryCast(string left, string name, string from, string to)
        {
            return $"try_cast(\"{left}\".\"{name}\" as {to})";
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
                .Append("update ")
                .Append(tableBracket)
                .Append(setClause);
            sqlWhereAppender(commandText);
            commandText
                .Append(" if @@rowcount = 0 insert into ")
                .Append(tableBracket)
                .Append($"({intoClause}) values({valueClause})");
            return commandText.ToString();
        }

        public string CreateFullTextWhereItem(string itemsTableName, string paramName)
        {
            return $"(contains(\"{itemsTableName}\".\"FullText\", @{paramName}#CommandCount#))";
        }

        public string CreateFullTextWhereBinary(
            string itemsTableName,
            string paramName)
        {
            return $"(exists(select * from \"Binaries\" where \"Binaries\".\"ReferenceId\"=\"{itemsTableName}\".\"ReferenceId\" and contains(\"Bin\", @{paramName}#CommandCount#)))";

        }

        public Dictionary<string,string> CreateSearchTextWords(
            Dictionary<string,string> words,
            string searchText)
        {
            return words;
        }
    }
}
