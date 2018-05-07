using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
using System.Web.UI;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [CheckContract]
    [ValidateInput(false)]
    [RefleshSiteInfo]
    public class ResourcesController : Controller
    {
        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ContentResult Scripts()
        {
            var log = new SysLogModel();
            var result = JavaScripts.Get();
            log.Finish(result.Content.Length);
            return result;
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ContentResult Styles()
        {
            var log = new SysLogModel();
            var result = Css.Get();
            log.Finish(result.Content.Length);
            return result;
        }
    }
}