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
        public static void Initialize()
        {
            var context = new Context(
                request: false,
                sessionStatus: false,
                sessionData: false,
                user: false,
                item: false)
            {
                TenantId = 1
            };
            var tenantModel = new TenantModel(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: context.TenantId);
            if (tenantModel.AccessStatus != Databases.AccessStatuses.Selected)
            {
                Repository.ExecuteNonQuery(
                    context: context,
                    connectionString: Parameters.Rds.OwnerConnectionString,
                    statements: new[] {
                        Rds.IdentityInsertTenants(factory: context, on: true),
                        Rds.InsertTenants(
                            param: Rds.TenantsParam()
                                .TenantId(context.TenantId)
                                .TenantName("DefaultTenant")),
                        Rds.IdentityInsertTenants(factory: context, on: false)
                    });
            }
        }
    }
}
