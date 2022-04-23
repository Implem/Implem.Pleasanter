using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using System.Net;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class ErrorsController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var context = new Context();
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.ApplicationError));
                return View();
            }
            else
            {
                return Content(Error.Types.ApplicationError.MessageJson(context: context));
            }
        }

        [AllowAnonymous]
        public ActionResult InvalidIpAddress()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            ViewBag.HtmlBody = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.InvalidIpAddress));
            return View();
        }

        [AllowAnonymous]
        public ActionResult BadRequest()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var context = new Context();
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.BadRequest));
                return View();
            }
            else
            {
                return Content(Error.Types.NotFound.MessageJson(context: context));
            }
        }

        [AllowAnonymous]
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            var context = new Context();
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.NotFound));
                return View();
            }
            else
            {
                return Content(Error.Types.NotFound.MessageJson(context: context));
            }
        }

        [AllowAnonymous]
        public ActionResult ParameterSyntaxError()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            var messageData = new string[]
            {
                Parameters.SyntaxErrors?.Join(",")
            };
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.ParameterSyntaxError),
                    messageData: messageData);
                return View();
            }
            else
            {
                return Content(Error.Types.ParameterSyntaxError.MessageJson(
                    context: context,
                    data: messageData));
            }
        }

        [AllowAnonymous]
        public ActionResult InternalServerError()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var context = new Context();
            if (!context.Ajax)
            {
                ViewBag.HtmlBody = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.InternalServerError));
                return View();
            }
            else
            {
                return Content(Error.Types.InternalServerError.MessageJson(context: context));
            }
        }

        [AllowAnonymous]
        public ActionResult LoginIdAlreadyUse()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            ViewBag.HtmlBody = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.LoginIdAlreadyUse));
            return View();
        }

        [AllowAnonymous]
        public ActionResult UserLockout()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            ViewBag.HtmlBody = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.UserLockout));
            return View();
        }

        [AllowAnonymous]
        public ActionResult UserDisabled()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            ViewBag.HtmlBody = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.UserDisabled));
            return View();
        }

        [AllowAnonymous]
        public ActionResult SamlLoginFailed()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            ViewBag.HtmlBody = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.SamlLoginFailed));
            return View();
        }

        [AllowAnonymous]
        public ActionResult InvalidSsoCode()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            ViewBag.HtmlBody = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.InvalidSsoCode));
            return View();
        }

        [AllowAnonymous]
        public ActionResult EmptyUserName()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            ViewBag.HtmlBody = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.EmptyUserName));
            return View();
        }
    }
}
