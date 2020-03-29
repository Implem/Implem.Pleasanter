using Implem.Pleasanter.NetFramework.Libraries.Requests;
using Implem.Pleasanter.NetFramework.Libraries.Responses;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
namespace Implem.Pleasanter.NetFramework.Controllers.Api
{
    [AllowAnonymous]
    public class ExtendedController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Sql(long id)
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body);
            var controller = new Pleasanter.Controllers.Api.ExtendedController();
            var result = controller.Sql(context: context);
            return result.ToHttpResponse(Request);
        }
   }
}