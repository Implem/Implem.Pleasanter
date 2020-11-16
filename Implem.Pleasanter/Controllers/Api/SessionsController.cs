using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Controllers.Api
{
    public class SessionsController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Get()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? SessionUtilities.GetByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }
        
        [HttpPost]
        public async Task<HttpResponseMessage> Set()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? SessionUtilities.SetByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }
        
        [HttpPost]
        public async Task<HttpResponseMessage> Delete()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? SessionUtilities.DeleteByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }
    }
}