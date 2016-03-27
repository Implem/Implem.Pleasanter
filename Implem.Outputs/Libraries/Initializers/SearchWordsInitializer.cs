using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.ServerData;
using System;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class SearchWordsInitializer
    {
        public static void Initialize()
        {
            if (Rds.ExecuteScalar_int(statements:
                Rds.SelectSearchWords(column: Rds.SearchWordsColumn().SearchWordsCount())) == 0)
            {
                SiteInfo.ItemUpdatedCollection.AddRange(Rds.ExecuteTable(statements:
                    Rds.SelectItems(column: Rds.ItemsColumn().ReferenceId()))
                        .AsEnumerable()
                        .Select(o =>
                            o["ReferenceId"].ToLong() + "," +
                            DateTime.Now.ToString("yyyy/M/d H:m:s.fff")));
            }
        }
    }
}