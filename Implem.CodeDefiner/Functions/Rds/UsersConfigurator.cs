using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.Rds
{
    internal static class UsersConfigurator
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
            Spids.Kill(factory: factory, uid: cn["uid"]);
            if (Exists(factory: factory, uid: cn["uid"], sql: Def.Sql.ExistsLoginRole))
            {
                Drop(factory: factory, uid: cn["uid"], sql: Def.Sql.DropLoginRole);
            }
            Create(factory: factory, uid: cn["uid"], sql: CreateLoginRoleCommandText(pwd: cn["pwd"]));
            if (Exists(factory: factory, uid: cn["uid"], sql: Def.Sql.ExistsUser))
            {
                // TODO　依存オブジェクトがあるとNG
                Drop(factory: factory, uid: cn["uid"], sql: Def.Sql.DropUser);
            }
            Create(factory: factory, uid: cn["uid"], sql: CreateUserCommandText(uid: cn["uid"], pwd: cn["pwd"]));
        }

        private static string CreateLoginRoleCommandText(string pwd)
        {
            return Def.Sql.CreateLoginRole
                .Replace("#Pwd#", pwd);
        }

        private static string CreateUserCommandText(string uid, string pwd)
        {
            return uid.EndsWith("_Owner")
                ? Def.Sql.CreateLoginAdmin.Replace("#Pwd#", pwd)
                : Def.Sql.CreateLoginUser.Replace("#Pwd#", pwd);
        }

        private static void Create(ISqlObjectFactory factory, string uid, string sql)
        {
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                dbTransaction: null,
                dbConnection: null,
                commandText: sql.Replace("#Uid#", uid).Replace("#ServiceName#", Environments.ServiceName));
        }

        private static void Drop(ISqlObjectFactory factory, string uid, string sql)
        {
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                dbTransaction: null,
                dbConnection: null,
                commandText: sql.Replace("#Uid#", uid).Replace("#ServiceName#", Environments.ServiceName));
        }

        private static bool Exists(ISqlObjectFactory factory, string uid, string sql)
        {
            return Def.SqlIoBySa(factory).ExecuteTable(
                factory: factory,
                commandText: sql.Replace("#Uid#", uid).Replace("#ServiceName#", Environments.ServiceName))
                .Rows.Count == 1;
        }
    }
}
