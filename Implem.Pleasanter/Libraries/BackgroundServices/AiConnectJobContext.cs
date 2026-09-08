using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
namespace Implem.Pleasanter.Libraries.BackgroundServices
{
    public static class AiConnectJobContext
    {
        public static Context CreateContext(
            BackgroundJobModel backgroundJobModel,
            string language)
        {
            var lookupContext = new Context(
                tenantId: backgroundJobModel.TenantId,
                request: false);
            var user = SiteInfo.User(
                context: lookupContext,
                userId: backgroundJobModel.UserId);
            var context = new Context(
                tenantId: backgroundJobModel.TenantId,
                deptId: user.DeptId,
                userId: backgroundJobModel.UserId,
                language: language,
                request: false,
                setAuthenticated: true)
            {
                AbsoluteUri = Parameters.Service.AbsoluteUri
            };
            context.SetTenantProperties(force: true);
            return context;
        }

        public static SiteSettings GetSiteSettings(
            Context context,
            long siteId)
        {
            return SiteSettingsUtilities.Get(
                context: context,
                siteId: siteId);
        }

        public static AiProvider GetAiProvider(
            SiteSettings ss,
            int aiProviderId)
        {
            var aiProvider = ss?.AiProviders?.Get(aiProviderId);
            return aiProvider?.Disabled == true
                || ss?.AiProvidersAllDisabled == true
                    ? null
                    : aiProvider;
        }
    }
}
