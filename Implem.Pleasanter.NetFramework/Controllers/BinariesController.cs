using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class BinariesController : Controller
    {
        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult SiteImageThumbnail(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BinariesController();
            var fileContent = controller.SiteImageThumbnail(context: context, reference: reference, id: id);
            return fileContent;
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult SiteImageIcon(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BinariesController();
            var fileContent = controller.SiteImageIcon(context: context, reference: reference, id: id);
            return fileContent;
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult TenantImageLogo()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BinariesController();
            var fileContent = controller.TenantImageLogo(context: context);
            return fileContent;
        }

        [HttpPost]
        public string UpdateSiteImage(string reference, long id, HttpPostedFileBase[] file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Pleasanter.Controllers.BinariesController();
            var json = controller.UpdateSiteImage(context: context, reference: reference, id: id, file: Libraries.Requests.HttpPostedFile.Create(file));
            return json;
        }

        [HttpPost]
        public string UpdateTenantImage(HttpPostedFileBase[] file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Pleasanter.Controllers.BinariesController();
            var json = controller.UpdateTenantImage(context: context, file: Libraries.Requests.HttpPostedFile.Create(file));
            return json;
        }

        [HttpDelete]
        public string DeleteSiteImage(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BinariesController();
            var json = controller.DeleteSiteImage(context: context, reference: reference, id: id);
            return json;
        }

        [HttpDelete]
        public string DeleteTenantImage()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BinariesController();
            var json = controller.DeleteTenantImage(context: context);
            return json;
        }

        [HttpPost]
        public string UploadImage(string reference, long id, HttpPostedFileBase[] file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Pleasanter.Controllers.BinariesController();
            var json = controller.UploadImage(context: context, reference: reference, id: id, file: Libraries.Requests.HttpPostedFile.Create(file));
            return json;
        }

        [HttpDelete]
        public string DeleteImage(string reference, string guid)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BinariesController();
            var json = controller.DeleteImage(context: context, reference: reference, guid: guid);
            return json;
        }

        [HttpPost]
        public string MultiUpload(string reference, long id, HttpPostedFileBase[] file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Pleasanter.Controllers.BinariesController();
            var json = controller.MultiUpload(context: context, reference: reference, id: id, file: Libraries.Requests.HttpPostedFile.Create(file));
            return json;
        }

        [HttpGet]
        public ActionResult Download(string reference, string guid)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BinariesController();
            var file = controller.Download(context: context, reference: reference, guid: guid);
            if (file == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            return file;
        }

        [HttpGet]
        public ActionResult DownloadTemp(string reference, string guid)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BinariesController();
            var file = controller.DownloadTemp(context: context, reference: reference, guid: guid);
            if (file == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            return file;
        }

        [HttpGet]
        public ActionResult Show(string reference, string guid)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BinariesController();
            var file = controller.Show(context: context, reference: reference, guid: guid);
            if (file == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            return File(file.FileContents, file.ContentType);
        }

        [HttpGet]
        public ActionResult ShowTemp(string reference, string guid)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BinariesController();
            var fileContent = controller.ShowTemp(context: context, reference: reference, guid: guid);
            return fileContent != null
                ? File(fileContent.FileContents, fileContent.ContentType)
                : null;
        }

        [HttpPost]
        public string DeleteTemp(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.BinariesController();
            var json = controller.DeleteTemp(context: context, reference: reference, id: id);
            return json.ToString();
        }

        [HttpPost]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public object Upload(long id, HttpPostedFileBase[] file)
        {
            var context = new ContextImplement(files: file);
            var controller = new Pleasanter.Controllers.BinariesController();
            var contentRangeHeader = Request.Headers["Content-Range"];
            var matches = System.Text.RegularExpressions.Regex.Matches(contentRangeHeader ?? string.Empty, "\\d+");
            var contentRange = matches.Count > 0
                ? new System.Net.Http.Headers.ContentRangeHeaderValue(
                    long.Parse(matches[0].Value),
                    long.Parse(matches[1].Value),
                    long.Parse(matches[2].Value))
                : null;

            var json = controller.Upload(context: context, id: id, contentRange: contentRange);
            return json.ToString();
        }
    }
}