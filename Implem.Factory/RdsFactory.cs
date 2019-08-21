using System;
using Implem.IRds;
using Implem.SqlServer;
using Implem.PostgreSql;
namespace Implem.Factory
{
    public static class RdsFactory
    {
        public static ISqlObjectFactory Create(string dbms)
        {
            switch (dbms)
            {
                case "SQLServer":
                    return (ISqlObjectFactory)Activator.CreateInstance(typeof(SqlServerObjectFactory));
                case "PostgreSQL":
                    return (ISqlObjectFactory)Activator.CreateInstance(typeof(PostgreSqlObjectFactory));
                default:
                    throw new NotSupportedException($"DBMS[{dbms}] is not supported by Pleasanter.");
            }
        }
    }
}
