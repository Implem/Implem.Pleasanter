using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
using System.Web.UI;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    [ValidateInput(false)]
    public class ResourcesController : Controller
    {
        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ContentResult Scripts()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = JavaScripts.Get();
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ContentResult Styles()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = Css.Get();
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}