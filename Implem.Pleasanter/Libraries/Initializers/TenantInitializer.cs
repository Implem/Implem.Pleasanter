using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.Initializers
{
    public static class TenantInitializer
    {
        public static void Initialize(Context context)
        {
            var tenantId = 1;
            var tenantModel = new TenantModel(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: tenantId);
            if (tenantModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    connectionString: Parameters.Rds.OwnerConnectionString,
                    statements: new[] {
                        Rds.IdentityInsertTenants(factory: context, on: true),
                        Rds.InsertTenants(
                            param: Rds.TenantsParam()
                                .TenantId(tenantId)
                                .TenantName("DefaultTenant")),
                        Rds.IdentityInsertTenants(factory: context, on: false)
                    });
            }
        }
    }
}
