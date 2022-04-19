using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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
                var html = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.ApplicationError));
                ViewBag.HtmlBody = html;
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
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.InvalidIpAddress));
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public new ActionResult BadRequest()
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var context = new Context();
            if (!context.Ajax)
            {
                var html = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.BadRequest));
                ViewBag.HtmlBody = html;
                return View();
            }
            else
            {
                return Content(Error.Types.NotFound.MessageJson(context: context));
            }
        }

        [AllowAnonymous]
        public new ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            var context = new Context();
            if (!context.Ajax)
            {
                var html = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.NotFound));
                ViewBag.HtmlBody = html;
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
                var html = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.ParameterSyntaxError),
                    messageData: messageData);
                ViewBag.HtmlBody = html;
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
                var html = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.InternalServerError));
                ViewBag.HtmlBody = html;
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
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.LoginIdAlreadyUse));
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public ActionResult UserLockout()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.UserLockout));
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public ActionResult UserDisabled()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.UserDisabled));
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public ActionResult SamlLoginFailed()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.SamlLoginFailed));
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public ActionResult InvalidSsoCode()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.InvalidSsoCode));
            ViewBag.HtmlBody = html;
            return View();
        }

        [AllowAnonymous]
        public ActionResult EmptyUserName()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var context = new Context();
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.EmptyUserName));
            ViewBag.HtmlBody = html;
            return View();
        }
    }
}
