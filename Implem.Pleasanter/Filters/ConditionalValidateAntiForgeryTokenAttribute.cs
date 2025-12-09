using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class ConditionalValidateAntiForgeryTokenAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
    {
        if (filterContext == null)
        {
            throw new ArgumentNullException(nameof(filterContext));
        }

        var method = filterContext.HttpContext.Request.Method;
        if (HttpMethods.IsGet(method) ||
            HttpMethods.IsHead(method) ||
            HttpMethods.IsOptions(method) ||
            HttpMethods.IsTrace(method))
        {
            return;
        }

        var endpoint = filterContext.HttpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAntiforgeryPolicy>() is IAntiforgeryPolicy policy && policy is IgnoreAntiforgeryTokenAttribute)
        {
            return;
        }

        var context = new Context(
                                        sessionStatus: false,
                                        sessionData: false,
                                        item: false,
                                        setPermissions: false);

        if (!Parameters.Security.TokenCheck && !context.IsForm)
        {
            return;
        }

        try
        {
            var antiforgery = filterContext.HttpContext.RequestServices.GetRequiredService<IAntiforgery>();

            await antiforgery.ValidateRequestAsync(filterContext.HttpContext);
        }
        catch (AntiforgeryValidationException)
        {

            filterContext.HttpContext.Response.StatusCode = 400;

            var isAjaxRequest = filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjaxRequest)
            {
                filterContext.Result = new JsonResult(new
                {
                    Message = Displays.InvalidRequest(context: context)
                });
            }
            else
            {
                filterContext.Result = new ContentResult()
                {
                    Content = Displays.InvalidRequest(context: context)
                };
            }
        }
    }
}