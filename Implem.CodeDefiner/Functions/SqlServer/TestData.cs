using Implem.DefinitionAccessor;
using Implem.Libraries.DataSources.SqlServer;
using System;
namespace Implem.CodeDefiner.Functions.SqlServer
{
    public static class TestData
    {
        public static void Insert()
        {
            for (var i = 0; i < 5000; i++)
            {
                Def.SqlIoByAdmin(statements: new SqlStatement
                {
                    CommandText = Def.Sql.InsertTestData
                })
                .ExecuteNonQuery();
                Console.WriteLine(i);
            }
        }
    }
}
