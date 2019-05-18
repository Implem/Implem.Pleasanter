using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
namespace Implem.Pleasanter.Controllers.Api
{
    [AllowAnonymous]
    public class UsersController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Get()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new UserModel().GetByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Create()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new UserModel().CreateByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Update(int id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new UserModel().UpdateByApi(context: context, userId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new Context(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new UserModel().DeleteByApi(context: context, userId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(Request);
        }
    }
}