using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
namespace Implem.Pleasanter.Filters
{
    public class CheckApiContextAttributes : FilterAttribute, IAuthorizationFilter
    {
        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext,
            CancellationToken cancellationToken,
            Func<Task<HttpResponseMessage>> continuation)
        {
            var stream = await actionContext?.Request?.Content?.ReadAsStreamAsync();
            if (stream == null)
            {
                return await Task.FromResult(actionContext.Request.CreateResponse(
                    statusCode: HttpStatusCode.BadRequest,
                    value: new
                    {
                        Message = Displays.BadRequest(
                            context: new Context(
                                sessionStatus: false,
                                sessionData: false,
                                item: false))
                    },
                    mediaType: "application/json"));
            }
            var reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);
            var requestData = await reader.ReadToEndAsync();
            stream.Position = 0;
            var context = new Context(
                sessionStatus: false,
                sessionData: false,
                item: false,
                apiRequestBody: requestData);
            if (!context.ContractSettings.AllowedIpAddress(context.UserHostAddress))
            {
                return await Task.FromResult(actionContext.Request.CreateResponse(
                    statusCode: HttpStatusCode.Forbidden,
                    value: new
                    {
                        Message = Displays.InvalidIpAddress(context)
                    },
                    mediaType: "application/json"));
            }
            if (Parameters.Security.TokenCheck
                && HttpContext.Current?.User?.Identity?.IsAuthenticated == true)
            {
                var data = await actionContext.Request?.Content?.ReadAsStringAsync();
                var api = data?.Deserialize<Api>();
                if (api?.Token != Authentications.Token())
                {
                    return await Task.FromResult(actionContext.Request.CreateResponse(
                    statusCode: HttpStatusCode.BadRequest,
                    value: new
                    {
                        Message = Displays.BadRequest(context: context)
                    },
                    mediaType: "application/json"));
                }
            }
            return await continuation();
        }
    }
}