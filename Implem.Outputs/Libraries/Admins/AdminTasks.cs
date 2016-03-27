using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace Implem.Pleasanter.Libraries.Admins
{
    public static class AdminTasks
    {
        public static string Do()
        {
            var now = DateTime.Now;
            while ((DateTime.Now - now).Seconds <= Parameters.AdminTasksDoSpan)
            {
                var processed = new List<string>();
                SiteInfo.ItemUpdatedCollection.ToList().ForEach(data =>
                {
                    Search.CreateIndex(data.Split_1st().ToLong());
                    processed.Add(data);
                });
                SiteInfo.ItemUpdatedCollection.RemoveAll(o => processed.Contains(o));
                Thread.Sleep(100);
            }
            return new ResponseCollection().ToJson();
        }
    }
}
