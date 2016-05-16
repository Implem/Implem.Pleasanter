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
            var html = PermissionsUtility.Editor(id);
            ViewBag.HtmlBody = html;
            log.Finish(html.Length);
            return View();
        }

        [HttpPut]
        public string ChangeInherit(string table, long id)
        {
            var log = new SysLogModel();
            var json = PermissionsUtility.ChangeInherit(id);
            log.Finish(json.Length);
            return json;
        }

        [AcceptVerbs(HttpVerbs.Put | HttpVerbs.Post | HttpVerbs.Delete)]
        public string Set(string table, long id)
        {
            var log = new SysLogModel();
            var json = PermissionsUtility.Set(id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Update(string table, long id)
        {
            var log = new SysLogModel();
            var json = PermissionsUtility.Update(id);
            log.Finish(json.Length);
            return json;
        }
    }
}
