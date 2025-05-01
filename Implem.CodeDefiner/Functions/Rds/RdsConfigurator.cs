using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.Rds
{
    internal static class RdsConfigurator
    {
        internal static void Configure(ISqlObjectFactory factory)
        {
            if (!Exists(factory: factory, databaseName: Environments.ServiceName))
            {
                CreateDatabase(factory: factory, databaseName: Environments.ServiceName);
            }
            else
            {
                UpdateDatabase(factory: factory, databaseName: Environments.ServiceName);
            }
        }

        private static void CreateDatabase(ISqlObjectFactory factory, string databaseName)
        {
            Consoles.Write(Environments.ServiceName, Consoles.Types.Info);
            var ocn = new TextData(Parameters.Rds.OwnerConnectionString, ';', '=');
            var ucn = new TextData(Parameters.Rds.UserConnectionString, ';', '=');
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                dbTransaction: null,
                dbConnection: null,
                commandText: Def.Sql.CreateDatabase
                    .Replace("#InitialCatalog#", databaseName));
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                dbTransaction: null,
                dbConnection: null,
                commandText: Def.Sql.CreateUserForPostgres
                    .Replace("#Uid_Owner#", ocn["uid"])
                    .Replace("#Pwd_Owner#", ocn["pwd"])
                    .Replace("#Uid_User#", ucn["uid"])
                    .Replace("#Pwd_User#", ucn["pwd"])
                    .Replace("#SchemaName#", factory.SqlDefinitionSetting.SchemaName));
            // PostgreSQL15対応:コマンドをわけないとエラーとなる為の処理
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                dbTransaction: null,
                dbConnection: null,
                commandText: Def.Sql.CreateDatabaseForPostgres
                    .Replace("#InitialCatalog#", databaseName)
                    .Replace("#Uid_Owner#", ocn["uid"]));
        }

        /// <summary>
        /// PostgreSQLの場合かつ、DBが既に存在する場合かつ、ユーザがない場合にはユーザの作成を行う。またDBのオーナ変更を行う。
        /// SQLServerおよびMySQLではダミーのSQLが実行される。
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="databaseName"></param>
        private static void UpdateDatabase(ISqlObjectFactory factory, string databaseName)
        {
            Consoles.Write(Environments.ServiceName, Consoles.Types.Info);
            var ocn = new TextData(Parameters.Rds.OwnerConnectionString, ';', '=');
            var ucn = new TextData(Parameters.Rds.UserConnectionString, ';', '=');
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                dbTransaction: null,
                dbConnection: null,
                commandText: Def.Sql.CreateUserForPostgres
                    .Replace("#Uid_Owner#", ocn["uid"])
                    .Replace("#Pwd_Owner#", ocn["pwd"])
                    .Replace("#Uid_User#", ucn["uid"])
                    .Replace("#Pwd_User#", ucn["pwd"])
                    .Replace("#SchemaName#", factory.SqlDefinitionSetting.SchemaName));
            Def.SqlIoBySa(factory).ExecuteNonQuery(
                factory: factory,
                dbTransaction: null,
                dbConnection: null,
                commandText: Def.Sql.ChangeDatabaseOwnerForPostgres
                    .Replace("#InitialCatalog#", databaseName)
                    .Replace("#Uid_Owner#", ocn["uid"]));
        }

        internal static bool Exists(ISqlObjectFactory factory, string databaseName)
        {
            var isExists = Def.SqlIoBySa(factory).ExecuteTable(
                factory: factory,
                commandText: Def.Sql.ExistsDatabase.Replace("#InitialCatalog#", databaseName))
                .Rows.Count == 1;
            var schemaName = isExists == false
                ? databaseName
                : (Def.SqlIoByAdmin(factory).ExecuteTable(
                    factory: factory,
                    commandText: Def.Sql.ExistsSchema.Replace("#SchemaName#", databaseName))
                        .Rows.Count == 1)
                            ? databaseName
                            : "public";
            factory.SqlDefinitionSetting.SchemaName = schemaName;
            if (!isExists) factory.SqlDefinitionSetting.IsCreatingDb = true;
            return isExists;
        }
    }
}
