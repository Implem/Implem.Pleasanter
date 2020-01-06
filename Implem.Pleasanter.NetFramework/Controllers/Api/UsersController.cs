using Implem.Pleasanter.NetFramework.Libraries.Requests;
using Implem.Pleasanter.NetFramework.Libraries.Responses;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
namespace Implem.Pleasanter.NetFramework.Controllers.Api
{
    [AllowAnonymous]
    public class UsersController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Get()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body);
            var controller = new Pleasanter.Controllers.Api.UsersController();
            var result = controller.Get(context: context);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Create()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body);
            var controller = new Pleasanter.Controllers.Api.UsersController();
            var result = controller.Create(context: context);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Update(int id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body);
            var controller = new Pleasanter.Controllers.Api.UsersController();
            var result = controller.Update(context: context, id: id);
            return result.ToHttpResponse(Request);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body);
            var controller = new Pleasanter.Controllers.Api.UsersController();
            var result = controller.Delete(context: context, id: id);
            return result.ToHttpResponse(Request);
        }

    }
}