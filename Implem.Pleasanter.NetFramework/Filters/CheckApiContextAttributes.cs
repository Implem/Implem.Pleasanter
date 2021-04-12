using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
namespace Implem.Pleasanter.NetFramework.Filters
{
    public class CheckApiContextAttributes : FilterAttribute, IAuthorizationFilter
    {
        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            if (actionContext?.Request?.Content == null)
            {
                return await Task.FromResult(actionContext.Request.CreateResponse(HttpStatusCode.BadRequest));
            }
            var requestData = await actionContext?.Request?.Content?.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: false,
                sessionData: false,
                item: false,
                apiRequestBody: requestData);
            if (!context.ContractSettings.AllowedIpAddress(context.UserHostAddress))
            {
                return await Task.FromResult(actionContext.Request.CreateResponse(HttpStatusCode.Forbidden));
            }
            if (Parameters.Security.TokenCheck
                && HttpContext.Current?.User?.Identity?.IsAuthenticated == true)
            {
                var data = await actionContext.Request?.Content?.ReadAsStringAsync();
                var api = data?.Deserialize<Api>();
                if (api?.Token != context.Token())
                {
                    return await Task.FromResult(actionContext.Request.CreateResponse(HttpStatusCode.BadRequest));
                }
            }
            return await continuation();
        }
    }
}