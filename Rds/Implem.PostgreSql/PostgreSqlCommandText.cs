using Implem.IRds;
using System;
using System.Text;

namespace Implem.PostgreSql
{
    class PostgreSqlCommandText : ISqlCommandText
    {
        public string CreateLimitClause(int limit)
        {
            return limit > 0 ? $" limit {limit} " : string.Empty;
        }

        public string CreateSelectIdentity(string template, string identityColumnName)
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

        public string CreateUpdateOrInsert(
            string tableBracket,
            string setClause,
            Action<StringBuilder> sqlWhereAppender,
            string intoAndValueClause)
        {
            var commandText = new StringBuilder();
            commandText
                .Append(" insert into ")
                .Append(tableBracket)
                .Append(intoAndValueClause)
                .Append(" ON CONFLICT ON CONSTRAINT ")
                .Append("\"")
                .Append(tableBracket.Replace("\"", ""))
                .Append("_pkey")
                .Append("\"")
                .Append(" DO ");
            commandText
                .Append("update ")
                .Append(setClause);
            return commandText.ToString();
        }
    }
}
