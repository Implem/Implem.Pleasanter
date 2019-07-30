using Implem.IRds;
namespace Implem.PostgreSql
{
    class PostgreSqlCommandText : ISqlCommandText
    {
        public string CreateSelectIdentity(string template, string identityColumnName)
        {
            return string.Format(template, identityColumnName);
        }

        public string CreateSelectStatementTerminator(bool selectIdentity)
        {
            return selectIdentity ? "" : ";";
        }
    }
}
