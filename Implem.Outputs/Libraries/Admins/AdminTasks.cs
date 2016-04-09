using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
namespace Implem.Pleasanter.Libraries.Admins
{
    public static class AdminTasks
    {
        public static string Do()
        {
            var now = DateTime.Now;
            while ((DateTime.Now - now).Seconds <= Def.Parameters.AdminTasksDoSpan)
            {
                GetUpdateTarget().ForEach(referenceId =>
                {
                    Search.CreateIndex(referenceId);
                    Update(referenceId);
                });
                Thread.Sleep(100);
            }
            return new ResponseCollection().ToJson();
        }

        private static List<long> GetUpdateTarget()
        {
            return Rds.ExecuteTable(statements:
                Rds.SelectItems(
                    top: 100,
                    column: Rds.ItemsColumn().ReferenceId(),
                    where: Rds.ItemsWhere().UpdateTarget(true)))
                        .AsEnumerable()
                        .Select(o => o["ReferenceId"].ToLong())
                        .ToList();
        }

        private static void Update(long referenceId)
        {
            Rds.ExecuteNonQuery(statements:
                Rds.UpdateItems(
                    param: Rds.ItemsParam().UpdateTarget(false),
                    where: Rds.ItemsWhere().ReferenceId(referenceId),
                    addUpdatedTimeParam: false,
                    addUpdatorParam: false));
        }
    }
}
