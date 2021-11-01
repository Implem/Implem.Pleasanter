using Implem.Pleasanter.NetCore.Filters;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Implem.Pleasanter.NetCore.Libraries.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.NetCore.Controllers
{
    [AllowAnonymous]
    [PublishesAttributes]
    public class PublishBinariesController : Controller
    {
        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ActionResult SiteImageThumbnail(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.PublishBinariesController();
            var file = controller.SiteImageThumbnail(context: context, reference: reference, id: id);
            return file.ToFileContentResult();
        }

        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ActionResult SiteImageIcon(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.PublishBinariesController();
            var file = controller.SiteImageIcon(context: context, reference: reference, id: id);
            return file.ToFileContentResult();
        }

        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ActionResult TenantImageLogo()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.PublishBinariesController();
            var file = controller.TenantImageLogo(context: context);
            return file.ToFileContentResult();
        }

        [HttpGet]
        public ActionResult Download(string guid)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.PublishBinariesController();
            var file = controller.Download(context: context, guid: guid);
            if (file == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            return ConvertToFileStreamResult(file);
        }

        [HttpGet]
        public ActionResult Show(string guid)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.PublishBinariesController();
            var file = controller.Show(context: context, guid: guid);
            if (file == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            return File(file.FileContents, file.ContentType, file.FileDownloadName);
        }

        private static ActionResult ConvertToFileStreamResult(System.Web.Mvc.FileResult file)
        {
            var streamResult = file as System.Web.Mvc.FileStreamResult;
            if (streamResult != null)
            {
                return new FileStreamResult(streamResult.FileStream, streamResult.ContentType) { FileDownloadName = streamResult.FileDownloadName };
            }
            else
            {
                var pathResult = file as System.Web.Mvc.FilePathResult;
                var filestream = new System.IO.FileStream(pathResult.FileName, System.IO.FileMode.Open);
                return new FileStreamResult(filestream, pathResult.ContentType) { FileDownloadName = pathResult.FileDownloadName };
            }
        }
    }
}