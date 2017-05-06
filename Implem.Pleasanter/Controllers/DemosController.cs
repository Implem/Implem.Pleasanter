using Implem.DefinitionAccessor;
using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [CheckContract]
    [ValidateInput(false)]
    [RefleshSiteInfo]
    public class DemosController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public string Register()
        {
            var log = new SysLogModel();
            if (Parameters.Service.Demo)
            {
                var json = DemoUtilities.Register();
                log.Finish(json.Length);
                return json;
            }
            else
            {
                log.Finish();
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            var log = new SysLogModel();
            if (Parameters.Service.Demo)
            {
                DemoUtilities.Login();
                log.Finish();
                return Redirect(Locations.Get());
            }
            else
            {
                log.Finish();
                return RedirectToAction("Errors", "NotFound");
            }
        }
    }
}
