using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    [PublishesAttributes]
    public class PublishBinariesController : Controller
    {
        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ActionResult SiteImageIcon(string reference, long id)
        {
            var context = new Context();
            if (reference.ToLower() == "items")
            {
                var (bytes, contentType) = BinaryUtilities.SiteImageIcon(
                    context: context,
                    siteModel: new SiteModel(context: context, siteId: id));
                return new FileContentResult(bytes, contentType);
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult Download(string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.Download(context: context, guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents?.Length ?? 0);
            var result = FileContentResults.FileStreamResult(file: file);
            if (result == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            return result;
        }

        [HttpGet]
        public ActionResult Show(string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.Download(
                context: context,
                guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents?.Length ?? 0);
            if (file == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            if (Implem.DefinitionAccessor.Parameters.BinaryStorage.IsContentPreviewable(file.ContentType))
            {
                var result = file.FileStream();
                return File(result.FileContents, result.ContentType);
            }
            return FileContentResults.FileStreamResult(file: file);
        }
    }
}