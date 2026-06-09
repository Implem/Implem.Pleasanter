using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class ParametersController : Controller
    {
        private readonly IHostApplicationLifetime Lifetime;

        public ParametersController(IHostApplicationLifetime lifetime)
        {
            Lifetime = lifetime;
        }

        [AcceptVerbs(HttpVerbs.Get, HttpVerbs.Post)]
        public ActionResult Edit()
        {
            var context = new Context();
            if (!context.Ajax)
            {
                var log = new SysLogModel(context: context);
                var html = ParameterUtilities.Editor(
                    context: context,
                    ss: SiteSettingsUtilities.ParametersSiteSettings(context: context));
                ViewBag.HtmlBody = html;
                log.Finish(context: context, responseSize: html.Length);
                return View();
            }
            else
            {
                var log = new SysLogModel(context: context);
                var json = ParameterUtilities.EditorJson(
                    context: context,
                    ss: SiteSettingsUtilities.ParametersSiteSettings(context: context),
                    parameterId: context.TenantId);
                log.Finish(context: context, responseSize: json.Length);
                return Content(json);
            }
        }

        [HttpPut]
        public string Update()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = ParameterUtilities.Update(
                context: context,
                ss: SiteSettingsUtilities.ParametersSiteSettings(context: context),
                parameterId: context.TenantId);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Restart()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            log.Finish(context: context, responseSize: 0);
            if (!context.CanRestart())
            {
                return Messages.ResponseHasNotPermission(context: context).ToJson();
            }
            ParameterUtilities.RequestTenantRestart(context: context);
            Task.Run(async () =>
            {
                await Task.Delay(2000);
                Lifetime.StopApplication();
            });
            return Messages.ResponseRestarting(context: context).ToJson();
        }
    }
}
