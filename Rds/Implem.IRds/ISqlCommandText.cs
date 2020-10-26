using System;
using System.Collections.Generic;
using System.Text;
namespace Implem.IRds
{
    public interface ISqlCommandText
    {
        string CreateSelectIdentity(
            string template,
            string identityColumnName);

        string CreateSelectStatementTerminator(bool selectIdentity);

        string CreateUpdateOrInsert(
            string tableBracket,
            string setClause,
            Action<StringBuilder> sqlWhereAppender,
            string intoClause,
            string valueClause);

        string CreateTopClause(int top);

        string CreateLimitClause(int limit);

        string CreateDelete(string template);

        string CreateRestore(string template);

        string CreateIdentityInsert(string template);

        string CreateTryCast(string left, string name, string from, string to);

        string CreateFullTextWhereItem(string itemsTableName, string paramName);

        string CreateFullTextWhereBinary(string itemsTableName, string paramName);

        Dictionary<string,string> CreateSearchTextWords(Dictionary<string,string> words, string searchText);
    }
}
