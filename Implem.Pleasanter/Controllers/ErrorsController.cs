using System.Net;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
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
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult InternalServerError()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return View();
        }
    }
}
