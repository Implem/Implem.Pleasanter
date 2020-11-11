using Implem.Pleasanter.NetFramework.Libraries.Requests;
using Implem.Pleasanter.NetFramework.Libraries.Responses;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
namespace Implem.Pleasanter.NetFramework.Controllers.Api
{
    [AllowAnonymous]
    public class SessionsController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Get()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var controller = new Pleasanter.Controllers.Api.SessionsController();
            var result = controller.Get(context: context);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Set()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var controller = new Pleasanter.Controllers.Api.SessionsController();
            var result = controller.Set(context: context);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Delete()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var controller = new Pleasanter.Controllers.Api.SessionsController();
            var result = controller.Delete(context: context);
            return result.ToHttpResponse(Request);
        }
    }
}