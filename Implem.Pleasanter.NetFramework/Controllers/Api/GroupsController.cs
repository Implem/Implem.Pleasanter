using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.NetFramework.Libraries.Requests;
using Implem.Pleasanter.NetFramework.Libraries.Responses;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Implem.Pleasanter.NetFramework.Controllers.Api
{
    [AllowAnonymous]
    public class GroupsController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Get()
        {
            var body = await Request.Content.ReadAsStringAsync();
            var context = new ContextImplement(sessionStatus: false, sessionData: false, apiRequestBody: body);
            var controller = new Implem.Pleasanter.Controllers.Api.GroupsController();
            var result = controller.Get(context: context);
            return result.ToHttpResponse(Request);
        }
    }
}