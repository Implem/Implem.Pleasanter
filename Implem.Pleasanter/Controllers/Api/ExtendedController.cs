using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
namespace Implem.Pleasanter.Controllers.Api
{
    [AllowAnonymous]
    public class ExtendedController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Sql()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? ExtensionUtilities.Sql(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }
    }
}