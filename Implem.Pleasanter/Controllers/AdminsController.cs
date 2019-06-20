using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Initializers;
using Implem.Pleasanter.Libraries.Migrators;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class AdminsController
    {
        public string Index(Context context)
        {
            var log = new SysLogModel(context: context);
            var html = new HtmlBuilder().AdminsIndex(context: context);
            log.Finish(context: context, responseSize: html.Length);
            return html;
        }

        public void InitializeItems(Context context)
        {
            var log = new SysLogModel(context: context);
            ItemsInitializer.Initialize(context: context);
            log.Finish(context: context, responseSize: 0);
        }

        public void MigrateSiteSettings(Context context)
        {
            var log = new SysLogModel(context: context);
            SiteSettingsMigrator.Migrate(context: context);
            log.Finish(context: context, responseSize: 0);
        }

        public string ReloadParameters(Context context)
        {
            var log = new SysLogModel(context: context);
            var json = ParametersInitializer.Initialize(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
