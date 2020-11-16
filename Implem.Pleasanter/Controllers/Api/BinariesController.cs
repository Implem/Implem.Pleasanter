using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Http;
using System.Net.Http;
using Implem.Pleasanter.Libraries.Responses;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Controllers.Api
{
    [AllowAnonymous]
    public class BinariesController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Get(string guid)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? BinaryUtilities.ApiDonwload(
                    context: context,
                    guid: guid)
                : ApiResults.Unauthorized(context: context);
            log.Finish(
                context: context,
                responseSize: result?.Content.Length ?? 0);
            return result.ToHttpResponse(Request);
        }
    }
}