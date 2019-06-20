using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [AllowAnonymous]
    public class Api_GroupsController : Controller
    {
        [HttpPost]
        public ContentResult Get()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.Api_GroupsController();
            var result = controller.Get(context: context);
            return result;
        }
    }
}