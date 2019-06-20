using Implem.DefinitionAccessor;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    internal static class LoginsConfigurator
    {
        internal static void Configure()
        {
            Execute(Parameters.Rds.OwnerConnectionString);
            Execute(Parameters.Rds.UserConnectionString);
        }

        private static void Execute(string connectionString)
        {
            var cn = new TextData(connectionString, ';', '=');
            Consoles.Write(cn["uid"], Consoles.Types.Info);
            Spids.Kill(cn["uid"]);
            Def.SqlIoBySa().ExecuteNonQuery(
                CommandText(cn["uid"])
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
