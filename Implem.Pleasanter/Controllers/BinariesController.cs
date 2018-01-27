using Implem.Libraries.Utilities;
using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [CheckContract]
    [ValidateInput(false)]
    [RefleshSiteInfo]
    public class BinariesController : Controller
    {
        [HttpGet]
        [OutputCache(Duration = 86400)]
        public ActionResult SiteImageThumbnail(string reference, long id)
        {
            if (reference.ToLower() == "items")
            {
                var bytes = BinaryUtilities.SiteImageThumbnail(new SiteModel(id));
                return new FileContentResult(bytes, "image/png");
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        [OutputCache(Duration = 86400)]
        public ActionResult SiteImageIcon(string reference, long id)
        {
            if (reference.ToLower() == "items")
            {
                var bytes = BinaryUtilities.SiteImageIcon(new SiteModel(id));
                return new FileContentResult(bytes, "image/png");
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public string UpdateSiteImage(string reference, long id)
        {
            var log = new SysLogModel();
            var json = reference.ToLower() == "items"
                ? BinaryUtilities.UpdateSiteImage(new SiteModel(id))
                : new ResponseCollection().ToJson();
            log.Finish(0);
            return json;
        }

        [HttpPost]
        public string UploadImage(string reference, long id, HttpPostedFileBase[] file)
        {
            var log = new SysLogModel();
            var json = BinaryUtilities.UploadImage(file, id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string MultiUpload(string reference, long id, HttpPostedFileBase[] file)
        {
            var log = new SysLogModel();
            var json = BinaryUtilities.MultiUpload(file, id);
            log.Finish(json.Length);
            return json;
        }

        [HttpGet]
        public FileContentResult Download(string reference, string guid)
        {
            var log = new SysLogModel();
            var file = BinaryUtilities.Donwload(guid);
            log.Finish(file?.FileContents.Length ?? 0);
            return file;
        }

        [HttpGet]
        public FileContentResult DownloadTemp(string reference, string guid)
        {
            var log = new SysLogModel();
            var file = BinaryUtilities.DownloadTemp(guid);
            log.Finish(file?.FileContents.Length ?? 0);
            return file;
        }

        [HttpGet]
        public ActionResult Show(string reference, string guid)
        {
            var log = new SysLogModel();
            var file = BinaryUtilities.Donwload(guid);
            log.Finish(file?.FileContents.Length ?? 0);
            return file != null
                ? File(file.FileContents, file.ContentType)
                : null;
        }

        [HttpGet]
        public ActionResult ShowTemp(string reference, string guid)
        {
            var log = new SysLogModel();
            var file = BinaryUtilities.DownloadTemp(guid);
            log.Finish(file?.FileContents.Length ?? 0);
            return file != null
                ? File(file.FileContents, file.ContentType)
                : null;
        }

        [HttpPost]
        public string DeleteTemp(string reference, long id)
        {
            var log = new SysLogModel();
            var json = BinaryUtilities.DeleteTemp();
            log.Finish(json.Length);
            return json.ToString();
        }
    }
}