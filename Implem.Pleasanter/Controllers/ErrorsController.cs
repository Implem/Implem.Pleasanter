using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.HtmlParts;
using System.Net;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [RefleshSiteInfo]
    public class ErrorsController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            ViewBag.HtmlBody = HtmlTemplates.Error(Libraries.General.Error.Types.NotFound);
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult InternalServerError()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ViewBag.HtmlBody = HtmlTemplates.Error(
                Libraries.General.Error.Types.InternalServerError);
            return View();
        }
    }
}
