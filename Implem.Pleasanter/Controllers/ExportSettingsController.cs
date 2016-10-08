using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class ExportSettingsController : Controller
    {
        [HttpPut]
        public string Edit(string reference, long id)
        {
            var log = new SysLogModel();
            var json = ExportSettingUtilities.EditorJson(
                SiteInfo.IndexReferenceType(reference, id), id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Change(string reference, long id)
        {
            var log = new SysLogModel();
            var json = ExportSettingUtilities.Change();
            log.Finish(json.Length);
            return json;
        }

        [AcceptVerbs(HttpVerbs.Put | HttpVerbs.Post)]
        public string Set(string reference, long id)
        {
            var log = new SysLogModel();
            var json = new ExportSettingModel(
                Permissions.GetBySiteId(id),
                SiteInfo.IndexReferenceType(reference, id), id)
                    .Set();
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string UpdateOrCreate(string reference, long id)
        {
            var log = new SysLogModel();
            var json = ExportSettingUtilities.UpdateOrCreate(
                permissionType: Permissions.GetBySiteId(id),
                referenceType: SiteInfo.IndexReferenceType(reference, id),
                referenceId: id);
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(string reference, long id)
        {
            var log = new SysLogModel();
            var json = ExportSettingUtilities.Delete(
                referenceType: SiteInfo.IndexReferenceType(reference, id),
                referenceId: id);
            log.Finish(json.Length);
            return json;
        }
    }
}