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
            var responseCollection = ExportSettingsUtility.Edit(
                SiteInfo.IndexReferenceType(reference, id), id);
            log.Finish(responseCollection.Length);
            return responseCollection;
        }

        [HttpPut]
        public string Change(string reference, long id)
        {
            var log = new SysLogModel();
            var responseCollection = ExportSettingsUtility.Change();
            log.Finish(responseCollection.Length);
            return responseCollection;
        }

        [AcceptVerbs(HttpVerbs.Put | HttpVerbs.Post)]
        public string Set(string reference, long id)
        {
            var log = new SysLogModel();
            var responseCollection = new ExportSettingModel(
                Permissions.GetBySiteId(id),
                SiteInfo.IndexReferenceType(reference, id), id)
                    .Set();
            log.Finish(responseCollection.Length);
            return responseCollection;
        }

        [HttpPut]
        public string UpdateOrCreate(string reference, long id)
        {
            var log = new SysLogModel();
            var responseCollection = new ExportSettingModel(
                Permissions.GetBySiteId(id),
                SiteInfo.IndexReferenceType(reference, id), id, withTitle: true)
                    .UpdateOrCreate();
            log.Finish(responseCollection.Length);
            return responseCollection;
        }

        [HttpDelete]
        public string Delete(string reference, long id)
        {
            var log = new SysLogModel();
            var responseCollection = new ExportSettingModel(
                Permissions.GetBySiteId(id),
                SiteInfo.IndexReferenceType(reference, id), id, withTitle: true)
                    .Delete(redirect: false);
            log.Finish(responseCollection.Length);
            return responseCollection;
        }
    }
}