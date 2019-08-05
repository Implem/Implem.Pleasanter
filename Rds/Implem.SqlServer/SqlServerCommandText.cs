using Implem.IRds;
using System;
using System.Text;

namespace Implem.SqlServer
{
    class SqlServerCommandText : ISqlCommandText
    {
        public string CreateLimitClause(int limit)
        {
            return string.Empty;
        }

        public string CreateSelectIdentity(string template, string identityColumnName)
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

        public string CreateUpdateOrInsert(
            string tableBracket,
            string setClause,
            Action<StringBuilder> sqlWhereAppender,
            string intoAndValueClause)
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
                .Append(intoAndValueClause);
            return commandText.ToString();
        }
    }
}
