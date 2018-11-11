using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class DemosController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public string Register()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (Parameters.Service.Demo)
            {
                var json = DemoUtilities.Register(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
            else
            {
                log.Finish(context: context);
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            if (Parameters.Service.Demo)
            {
                DemoUtilities.Login(context: context);
                log.Finish(context: context);
                return Redirect(Locations.Get(context: context));
            }
            else
            {
                log.Finish(context: context);
                return RedirectToAction("Errors", "NotFound");
            }
        }
    }
}
