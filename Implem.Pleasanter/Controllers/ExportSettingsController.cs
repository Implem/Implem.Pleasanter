using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Libraries.Utilities;
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
            var json = ExportSettingsUtility.Edit(
                SiteInfo.IndexReferenceType(reference, id), id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Change(string reference, long id)
        {
            var log = new SysLogModel();
            var json = ExportSettingsUtility.Change();
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
            var json = new ExportSettingModel(
                Permissions.GetBySiteId(id),
                SiteInfo.IndexReferenceType(reference, id), id, withTitle: true)
                    .UpdateOrCreate();
            log.Finish(json.Length);
            return json;
        }

        [HttpDelete]
        public string Delete(string reference, long id)
        {
            var log = new SysLogModel();
            var json = new ExportSettingModel(
                Permissions.GetBySiteId(id),
                SiteInfo.IndexReferenceType(reference, id), id, withTitle: true)
                    .Delete(redirect: false);
            log.Finish(json.Length);
            return json;
        }
    }
}