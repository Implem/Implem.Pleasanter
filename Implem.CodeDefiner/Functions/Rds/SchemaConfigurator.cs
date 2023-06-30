﻿using Implem.DefinitionAccessor;
using Implem.IRds;
using Implem.Libraries.Classes;
using Implem.Libraries.Utilities;
namespace Implem.CodeDefiner.Functions.Rds
{
    internal static class SchemaConfigurator
    {
        internal static void Configure(ISqlObjectFactory factory)
        {
            if (factory.SqlDefinitionSetting.IsCreatingDb)
            {
                var ocn = new TextData(Parameters.Rds.OwnerConnectionString, ';', '=');
                var ucn = new TextData(Parameters.Rds.UserConnectionString, ';', '=');

                // DB内にスキーマを作りたいので、Implem.Pleasanterデータベースに接続しないといけない
                Def.SqlIoByAdmin(factory).ExecuteNonQuery(
                    factory: factory,
                    dbTransaction: null,
                    dbConnection: null,
                    commandText: Def.Sql.CreateSchema
                        .Replace("#Uid_Owner#", ocn["uid"])
                        .Replace("#Uid_User#", ucn["uid"])
                        .Replace("#ServiceName#", Environments.ServiceName)
                    );
            }
        }
    }
}
