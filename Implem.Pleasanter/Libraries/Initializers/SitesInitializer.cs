using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Server;
using System.Data;
using System.Linq;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class SitesInitializer
    {
        public static void Initialize()
        {
            Rds.ExecuteTable(statements: Rds.SelectSites(
                column: Rds.SitesColumn()
                    .TenantId()
                    .SiteId()
                    .ReferenceType()
                    .ParentId()
                    .Title()))
                        .AsEnumerable()
                        .ForEach(dataRow =>
                            SiteInfo.SiteMenu.Add(
                                dataRow["SiteId"].ToLong(),
                                new SiteMenuElement(
                                    dataRow["TenantId"].ToInt(),
                                    dataRow["SiteId"].ToLong(),
                                    dataRow["ReferenceType"].ToString(),
                                    dataRow["ParentId"].ToLong(),
                                    dataRow["Title"].ToString())));
        }
    }
}