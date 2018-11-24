using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Controllers.Api
{
    [AllowAnonymous]
    public class ItemsController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Get(long id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new ItemModel(context: context, referenceId: id).GetByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Create(long id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new ItemModel(context: context, referenceId: id).CreateByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Update(long id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new ItemModel(context: context, referenceId: id).UpdateByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Delete(long id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new ItemModel(context: context, referenceId: id).DeleteByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }
    }
}