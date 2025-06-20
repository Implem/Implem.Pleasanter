using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
using System.Collections.Generic;
using System.Linq;
namespace Implem.CodeDefiner.Functions.Rds
{
    internal static class UsersConfigurator
    {
        internal static void Configure(ISqlObjectFactory factory)
        {
            // MySqlConnectingHostはMySQLの場合のみSQLに反映が必要かつ2件以上の可能性がある
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

        private static void Execute(ISqlObjectFactory factory, string connectionString, string host = "")
        {
            var cn = new TextData(connectionString, ';', '=');
            Consoles.Write(cn["uid"], Consoles.Types.Info);
            if (Exists(
                factory: factory,
                uid: cn["uid"],
                sql: Def.Sql.ExistsUser,
                host: host))
            {
                Alter(
                    factory: factory,
                    uid: cn["uid"],
                    sql: AlterLoginRoleCommandText(pwd: cn["pwd"], host: host));
            }
            else
            {
                Create(
                    factory: factory,
                    uid: cn["uid"],
                    sql: CreateUserCommandText(uid: cn["uid"], pwd: cn["pwd"], host: host));
            }
        }

        private static string AlterLoginRoleCommandText(string pwd, string host)
        {
            return Def.Sql.AlterLoginRole
                .Replace("#Pwd#", pwd)
                .Replace("#MySqlConnectingHost#", host);
        }

        private static string CreateUserCommandText(string uid, string pwd, string host)
        {
            return uid.EndsWith("_Owner")
                ? Def.Sql.CreateLoginAdmin
                    .Replace("#Pwd#", pwd)
                    .Replace("#MySqlConnectingHost#", host)
                : Def.Sql.CreateLoginUser
                    .Replace("#Pwd#", pwd)
                    .Replace("#MySqlConnectingHost#", host);
        }

        private static void Create(ISqlObjectFactory factory, string uid, string sql)
        {
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                dbTransaction: null,
                dbConnection: null,
                commandText: sql.Replace("#Uid#", uid).Replace("#ServiceName#", Environments.ServiceName));
        }

        private static void Alter(ISqlObjectFactory factory, string uid, string sql)
        {
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                dbTransaction: null,
                dbConnection: null,
                commandText: sql.Replace("#Uid#", uid).Replace("#ServiceName#", Environments.ServiceName));
        }

        private static bool Exists(ISqlObjectFactory factory, string uid, string sql, string host)
        {
            return Def.SqlIoBySa(factory).ExecuteTable(
                factory: factory,
                commandText: sql
                    .Replace("#Uid#", uid)
                    .Replace("#ServiceName#", Environments.ServiceName)
                    .Replace("#MySqlConnectingHost#", host))
                .Rows.Count == 1;
        }

        internal static void KillTask(ISqlObjectFactory factory)
        {
            KillTask(
                factory: factory,
                connectionString: Parameters.Rds.OwnerConnectionString);
            KillTask(
                factory: factory,
                connectionString: Parameters.Rds.UserConnectionString);
        }

        private static void KillTask(ISqlObjectFactory factory, string connectionString)
        {
            var cn = new TextData(connectionString, ';', '=');
            Spids.Kill(factory: factory, uid: cn["uid"]);
        }
    }
}
