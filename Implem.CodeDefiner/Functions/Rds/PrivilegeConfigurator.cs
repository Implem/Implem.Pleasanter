using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.Rds
{
    internal static class PrivilegeConfigurator
    {
        internal static void Configure(ISqlObjectFactory factory)
        {
            if (Environments.RdsProvider != "Local") return;
            Execute(
                factory: factory,
                connectionString: Parameters.Rds.OwnerConnectionString);
            Execute(
                factory: factory,
                connectionString: Parameters.Rds.UserConnectionString);
        }

        private static void Execute(ISqlObjectFactory factory, string connectionString)
        {
            var cn = new TextData(connectionString, ';', '=');
            Consoles.Write(cn["uid"], Consoles.Types.Info);
            if (cn["uid"].EndsWith("_Owner"))
            {
                Def.SqlIoBySa(factory).ExecuteNonQuery(
                    factory: factory,
                    dbTransaction: null,
                    dbConnection: null,
                    commandText: 
                        Def.Sql.GrantPrivilegeAdmin
                        .Replace("#Uid#", cn["uid"])
                        .Replace("#ServiceName#", Environments.ServiceName));
            }
            else
            {
                Def.SqlIoByAdmin(factory).ExecuteNonQuery(
                    factory: factory,
                    dbTransaction: null,
                    dbConnection: null,
                    commandText:
                        Def.Sql.GrantPrivilegeUser
                        .Replace("#Uid#", cn["uid"])
                        .Replace("#ServiceName#", Environments.ServiceName));
            }
        }

        private static string CommandText(string uid)
        {
            return (uid.EndsWith("_Owner")
                ? Def.Sql.GrantPrivilegeAdmin.Replace("#Uid#", uid)
                : Def.Sql.GrantPrivilegeUser.Replace("#Uid#", uid))
                .Replace("#ServiceName#", Environments.ServiceName);
        }
    }
}
