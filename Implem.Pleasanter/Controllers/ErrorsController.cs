using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.HtmlParts;
using Implem.Pleasanter.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class ErrorsController
    {
        public string Index(Context context)
        {
            if (!context.Ajax)
            {
                var html = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.ApplicationError));
                return html;
            }
            else
            {
                return Error.Types.ApplicationError.MessageJson(context: context);
            }
        }

        public string InvalidIpAddress(Context context)
        {
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.InvalidIpAddress));
            return html;
        }

        public string BadRequest(Context context)
        {
            // Response.StatusCode = (int)HttpStatusCode.BadRequest;
            if (!context.Ajax)
            {
                var html = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.BadRequest));
                return html;
            }
            else
            {
                return Error.Types.NotFound.MessageJson(context: context);
            }
        }

        public string NotFound(Context context)
        {
            // Response.StatusCode = (int)HttpStatusCode.NotFound;
            if (!context.Ajax)
            {
                var html = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.NotFound));
                return html;
            }
            else
            {
                return Error.Types.NotFound.MessageJson(context: context);
            }
        }

        public string ParameterSyntaxError(Context context)
        {
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
                return html;
            }
            else
            {
                return Error.Types.ParameterSyntaxError.MessageJson(
                    context: context,
                    data: messageData);
            }
        }

        public string InternalServerError(Context context)
        {
            // Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            if (!context.Ajax)
            {
                var html = HtmlTemplates.Error(
                    context: context,
                    errorData: new ErrorData(type: Error.Types.InternalServerError));
                return html;
            }
            else
            {
                return Error.Types.InternalServerError.MessageJson(context: context);
            }
        }

        public string LoginIdAlreadyUse(Context context)
        {
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.LoginIdAlreadyUse));
            return html;
        }

        public string UserLockout(Context context)
        {
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.UserLockout));
            return html;
        }

        public string UserDisabled(Context context)
        {
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.UserDisabled));
            return html;
        }

        public string SamlLoginFailed(Context context)
        {
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.SamlLoginFailed));
            return html;
        }

        public string InvalidSsoCode(Context context)
        {
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.InvalidSsoCode));
            return html;
        }

        public string EmptyUserName(Context context)
        {
            var html = HtmlTemplates.Error(
                context: context,
                errorData: new ErrorData(type: Error.Types.EmptyUserName));
            return html;
        }
    }
}
