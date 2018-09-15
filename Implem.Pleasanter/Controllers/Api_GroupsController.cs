using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    public class Api_GroupsController : Controller
    {
        [HttpPost]
        public ContentResult Get()
        {
            var context = new Context(api: true);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new GroupModel().GetByApi(context: context)
                : ApiResults.Unauthorized();
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}