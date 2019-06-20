using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal static class LoginsConfigurator
    {
        internal static void Configure(ISqlObjectFactory factory)
        {
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
            Spids.Kill(factory: factory, uid: cn["uid"]);
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                commandText: CommandText(cn["uid"])
                    .Replace("#Uid#", cn["uid"])
                    .Replace("#Pwd#", cn["pwd"])
                    .Replace("#ServiceName#", Environments.ServiceName));
        }

        private static string CommandText(string uid)
        {
            return uid.EndsWith("_Owner")
                ? Def.Sql.RecreateLoginAdmin
                : Def.Sql.RecreateLoginUser;
        }
    }
}
