using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds
{
    internal static class PrivilegeConfigurator
    {
        internal static void Configure(ISqlObjectFactory factory)
        {
            var hostList = Parameters.Rds.Dbms == "MySQL"
                ? Parameters.Rds.MySqlConnectingHost.Split(',').ToList<string>()
                : new List<string>([""]);
            foreach (var host in hostList)
            {
                Execute(
                    factory: factory,
                    connectionString: Parameters.Rds.OwnerConnectionString,
                    host: host);
                Execute(
                    factory: factory,
                    connectionString: Parameters.Rds.UserConnectionString,
                    host: host);
            }
        }

        private static void Execute(ISqlObjectFactory factory, string connectionString, string host)
        {
            var cn = new TextData(connectionString, ';', '=');
            Consoles.Write(cn["uid"], Consoles.Types.Info);
            if (cn["uid"].EndsWith("_Owner"))
            {
                Def.SqlIoBySa(factory).ExecuteNonQuery(
                    factory: factory,
                    dbTransaction: null,
                    dbConnection: null,
                    commandText: Def.Sql.GrantPrivilegeAdmin
                        .Replace("#Uid#", cn["uid"])
                        .Replace("#ServiceName#", Environments.ServiceName)
                        .Replace("#SchemaName#", factory.SqlDefinitionSetting.SchemaName)
                        .Replace("#MySqlConnectingHost#", host));
            }
            else
            {
                var ocn = new TextData(Parameters.Rds.OwnerConnectionString, ';', '=');
                Def.SqlIoByAdmin(factory).ExecuteNonQuery(
                    factory: factory,
                    dbTransaction: null,
                    dbConnection: null,
                    commandText: Def.Sql.GrantPrivilegeUser
                        .Replace("#Uid#", cn["uid"])
                        .Replace("#Oid#", ocn["uid"])
                        .Replace("#ServiceName#", Environments.ServiceName)
                        .Replace("#SchemaName#", factory.SqlDefinitionSetting.SchemaName)
                        .Replace("#MySqlConnectingHost#", host));
            }
        }
    }
}
