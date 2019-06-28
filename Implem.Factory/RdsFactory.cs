using System;
using Implem.IRds;
using Implem.SqlServer;
using Implem.PostgreSql;
namespace Implem.Factory
{
    public static class RdsFactory
    {
        private const string SQLServer = "sqlserver";
        private const string PostgreSQL = "postgresql";

        public static ISqlObjectFactory Create(string dbms)
        {
            switch (dbms?.ToLower())
            {
                case SQLServer:
                    return (ISqlObjectFactory)Activator.CreateInstance(typeof(SqlServerObjectFactory));
                case PostgreSQL:
                    return (ISqlObjectFactory)Activator.CreateInstance(typeof(PostgreSqlObjectFactory));
                default:
                    throw new NotSupportedException($"DBMS[{dbms}] is not supported  by Pleasanter.");
            }
        }
    }
}
