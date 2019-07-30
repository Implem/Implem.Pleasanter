using Implem.IRds;
namespace Implem.SqlServer
{
    class SqlServerCommandText : ISqlCommandText
    {
        public string CreateSelectIdentity(string template, string identityColumnName)
        {
            return template;
        }

        public string CreateSelectStatementTerminator(bool selectIdentity)
        {
            return ";";
        }
    }
}
