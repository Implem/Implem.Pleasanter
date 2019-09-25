using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.NetCore.Controllers
{
    [Authorize]
    public class ErrorsController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var htmlOrJson = controller.Index(context: context);
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AllowAnonymous]
        public ActionResult InvalidIpAddress()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var html = controller.InvalidIpAddress(context: context);
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public ActionResult BadRequest()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var htmlOrJson = controller.BadRequest(context: context);
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AllowAnonymous]
        public ActionResult NotFound()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var htmlOrJson = controller.NotFound(context: context);
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AllowAnonymous]
        public ActionResult ParameterSyntaxError()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var htmlOrJson = controller.ParameterSyntaxError(context: context);
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AllowAnonymous]
        public ActionResult InternalServerError()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var htmlOrJson = controller.InternalServerError(context: context);
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = htmlOrJson;
                return View();
            }
            else
            {
                return Content(htmlOrJson);
            }
        }

        [AllowAnonymous]
        public ActionResult LoginIdAlreadyUse()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var html = controller.LoginIdAlreadyUse(context: context);
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public ActionResult UserLockout()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var html = controller.UserLockout(context: context);
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public ActionResult UserDisabled()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var html = controller.UserDisabled(context: context);
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public ActionResult SamlLoginFailed
()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var html = controller.SamlLoginFailed(context: context);
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public ActionResult InvalidSsoCode()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var html = controller.InvalidSsoCode(context: context);
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public ActionResult EmptyUserName()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ErrorsController();
            var html = controller.EmptyUserName(context: context);
            ViewBag.HtmlBody = html;
            return View();
        }
    }
}
