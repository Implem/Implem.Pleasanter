using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class PermissionsController : Controller
    {
        [HttpGet]
        public ActionResult Edit(string table, long id)
        {
            var log = new SysLogModel();
            var html = PermissionsUtility.Edit(id);
            ViewBag.HtmlBody = html;
            log.Finish(html.Length);
            return View();
        }

        [HttpPut]
        public string ChangeInherit(string table, long id)
        {
            var log = new SysLogModel();
            var responseCollection = PermissionsUtility.ChangeInherit(id);
            log.Finish(responseCollection.Length);
            return responseCollection;
        }

        [AcceptVerbs(HttpVerbs.Put | HttpVerbs.Post | HttpVerbs.Delete)]
        public string Set(string table, long id)
        {
            var log = new SysLogModel();
            var responseCollection = PermissionsUtility.Set(id);
            log.Finish(responseCollection.Length);
            return responseCollection;
        }

        [HttpPut]
        public string Update(string table, long id)
        {
            var log = new SysLogModel();
            var responseCollection = PermissionsUtility.Update(id);
            log.Finish(responseCollection.Length);
            return responseCollection;
        }
    }
}
