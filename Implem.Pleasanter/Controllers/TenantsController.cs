using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class TenantsController
    {
        public string Edit(Context context)
        {
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = TenantUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                    tenantId: context.TenantId,
                    clearSessions: true);
                log.Finish(context: context, responseSize: html.Length);
                return html;
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = TenantUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                    tenantId: context.TenantId);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
        }

        public string Update(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = TenantUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.TenantsSiteSettings(context: context),
                tenantId: context.TenantId);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
