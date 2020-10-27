using Implem.Pleasanter.NetFramework.Libraries.Requests;
using Implem.Pleasanter.NetFramework.Libraries.Responses;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
namespace Implem.Pleasanter.NetFramework.Controllers.Api
{
    [AllowAnonymous]
    public class ItemsController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Get(long id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var controller = new Pleasanter.Controllers.Api.ItemsController();
            var result = controller.Get(context: context, id: id);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Create(long id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var controller = new Pleasanter.Controllers.Api.ItemsController();
            var result = controller.Create(context: context, id: id);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Update(long id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var controller = new Pleasanter.Controllers.Api.ItemsController();
            var result = controller.Update(context: context, id: id);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Delete(long id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var controller = new Pleasanter.Controllers.Api.ItemsController();
            var result = controller.Delete(context: context, id: id);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> BulkDelete(long id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var controller = new Pleasanter.Controllers.Api.ItemsController();
            var result = controller.BulkDelete(context: context, id: id);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Export(long id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.Content.Headers.ContentType.MediaType);
            var controller = new Pleasanter.Controllers.Api.ItemsController();
            var result = controller.Export(context: context, id: id);
            return result.ToHttpResponse(Request);
        }
    }
}