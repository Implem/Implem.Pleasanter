using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using System.Text;
namespace Implem.PleasanterFilters
{
    public class CheckApiContextAttributes : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext.HttpContext?.Request?.Body == null)
            {
                filterContext.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                filterContext.Result = new JsonResult(
                    new
                    {
                        Message = Displays.BadRequest(context: new Context(
                            sessionStatus: false,
                            sessionData: false,
                            item: false))
                    });
                return;
            }
            filterContext.HttpContext.Request.EnableBuffering();
            var reader = new StreamReader(
                stream: filterContext.HttpContext.Request?.Body,
                encoding: Encoding.UTF8);
            var requestData = reader.ReadToEnd();
            filterContext.HttpContext.Request.Body.Position = 0;
            var context = new Context(
                sessionStatus: false,
                sessionData: false,
                item: false,
                setPermissions: false,
                apiRequestBody: requestData);
            if (!context.ContractSettings.AllowedIpAddress(context.UserHostAddress))
            {
                filterContext.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                filterContext.Result = new JsonResult(
                    new
                    {
                        Message = Displays.InvalidIpAddress(context: context)
                    });
                return;
            }
            if (Parameters.Security.TokenCheck
                && filterContext.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                var api = requestData?.Deserialize<Api>();
                if (api?.Token != context.Token())
                {
                    filterContext.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    filterContext.Result = new JsonResult(
                        new
                        {
                            Message = Displays.BadRequest(context: context)
                        });
                }
            }
        }
    }
}