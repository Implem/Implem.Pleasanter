using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [AllowAnonymous]
    [ConditionalValidateAntiForgeryToken]
    public class FormBinariesController : Controller
    {
        private readonly IAntiforgery _antiforgery;

        // コンストラクタで IAntiforgery をインジェクションする
        public FormBinariesController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        // ルートのパターン "{reference}/{guid}/{controller}/{action}"
        // new Context で guid が使用されるため、当アクションの引数としては指定不要。
        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ActionResult SiteImageIcon(string reference)
        {
            var context = new Context();
            var id = context.Id;
            if (reference.ToLower() == "forms")
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

        [HttpPost]
        public string UploadImage(string reference, string guid, ICollection<IFormFile> file)
        {
            var context = new Context(files: file);
            var id = context.Id;
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.UploadImage(
                context: context,
                id: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpDelete]
        public string DeleteImage(string reference, string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.DeleteImage(context: context, guid: guid);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpGet]
        public ActionResult Download(string reference, string guid)
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
        public ActionResult DownloadTemp(string reference, string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.DownloadTemp(context: context, guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents?.Length ?? 0);
            var result = FileContentResults.FileStreamResult(file: file);
            if (result == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            return result;
        }

        [HttpGet]
        public ActionResult Show(string reference, string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.Download(
                context: context,
                guid: guid)
                    ?.FileStream();
            log.Finish(context: context, responseSize: file?.FileContents?.Length ?? 0);
            var result = file != null
                ? new FileContentResult(file.FileContents, file.ContentType)
                : null;
            if (result == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            return File(result.FileContents, result.ContentType);
        }

        [HttpGet]
        public ActionResult ShowTemp(string reference, string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.DownloadTemp(
                context: context,
                guid: guid);
            var result = file?.FileStream();
            log.Finish(context: context, responseSize: result?.FileContents?.Length ?? 0);
            return result != null
                ? File(result.FileContents, result.ContentType)
                : null;
        }

        [HttpPost]
        public string DeleteTemp(string reference, string guidOfForm)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.DeleteTemp(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json.ToString();
        }

        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult Upload(string guidOfForm)
        {
            var files = Request.Form.Files;
            var context = new Context(files: files.ToList());
            var id = context.Id;
            var log = new SysLogModel(context: context);
            var contentRangeHeader = Request.Headers["Content-Range"].FirstOrDefault();
            var contentRange = BinaryUtilities.GetContentRange(contentRangeHeader: contentRangeHeader);
            var content = BinaryUtilities.UploadFile(context, id, contentRange);
            log.Finish(context: context, responseSize: content.Length);
            return new ContentResult()
            {
                Content = content,
                ContentType = "application/json",
            };
        }
    }
}