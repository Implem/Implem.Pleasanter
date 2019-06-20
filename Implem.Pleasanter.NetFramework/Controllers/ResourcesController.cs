using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
using System.Web.UI;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [AllowAnonymous]
    [ValidateInput(false)]
    public class ResourcesController : Controller
    {
        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ContentResult Scripts()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ResourcesController();
            var result = controller.Scripts(context: context);
            return result;
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ContentResult Styles()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.ResourcesController();
            var result = controller.Styles(context: context);
            return result;
        }
    }
}