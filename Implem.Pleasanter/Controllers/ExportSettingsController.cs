using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    [RefleshSiteInfo]
    public class ExportSettingsController : Controller
    {
        [HttpPut]
        public string Edit(string reference, long id)
        {
            var log = new SysLogModel();
            var json = ExportSettingUtilities.EditorJson(reference, id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Change(string reference, long id)
        {
            var log = new SysLogModel();
            var json = ExportSettingUtilities.Change(reference, id);
            log.Finish(json.Length);
            return json;
        }

        [AcceptVerbs(HttpVerbs.Put | HttpVerbs.Post)]
        public string Set(string reference, long id)
        {
            var log = new SysLogModel();
            var json = ExportSettingUtilities.Set(reference, id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string UpdateOrCreate(string reference, long id)
        {
            var log = new SysLogModel();
            var json = ExportSettingUtilities.UpdateOrCreate(reference, id);
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(string reference, long id)
        {
            var log = new SysLogModel();
            var json = ExportSettingUtilities.Delete(reference, id);
            log.Finish(json.Length);
            return json;
        }
    }
}