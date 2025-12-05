using Implem.Libraries.Utilities;
using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    public class BinariesController : Controller
    {
        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        [OutputCache(PolicyName = "imageCache")]
        public ActionResult SiteImageThumbnail(string reference, long id)
        {
            var context = new Context();
            if (reference.ToLower() == "items")
            {
                var (bytes, contentType) = BinaryUtilities.SiteImageThumbnail(
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
        [ResponseCache(Duration = int.MaxValue)]
        public ActionResult TenantImageLogo()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var (bytes, contentType) = BinaryUtilities.TenantImageLogo(
                context: context,
                tenantModel: new TenantModel(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context)));
            log.Finish(
                context: context,
                responseSize: bytes.Length);
            return new FileContentResult(bytes, contentType);
        }

        [HttpPost]
        public string UpdateSiteImage(string reference, long id, ICollection<IFormFile> file)
        {
            var context = new Context(files: file);
            var log = new SysLogModel(context: context);
            var json = reference.ToLower() == "items"
                ? BinaryUtilities.UpdateSiteImage(
                    context: context,
                    siteModel: new SiteModel(context: context, siteId: id))
                : new ResponseCollection(context: context).ToJson();
            log.Finish(context: context);
            return json;
        }

        [HttpPost]
        public string UpdateTenantImage(ICollection<IFormFile> file)
        {
            var context = new Context(files: file);
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context);
            var tenantModel = new TenantModel(context, ss).Get(context, ss);
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.UpdateTenantImage(
                context: context,
                tenantModel: tenantModel);
            log.Finish(context: context);
            return json;
        }

        [HttpDelete]
        public string DeleteSiteImage(string reference, long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = reference.ToLower() == "items"
                ? BinaryUtilities.DeleteSiteImage(
                    context: context,
                    siteModel: new SiteModel(context: context, siteId: id))
                : new ResponseCollection(context: context).ToJson();
            log.Finish(context: context);
            return json;
        }

        [HttpDelete]
        public string DeleteTenantImage()
        {
            var context = new Context();
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context);
            var tenantModel = new TenantModel(context, ss).Get(context, ss);
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.DeleteTenantImage(
                context: context,
                tenantModel: tenantModel);
            log.Finish(context: context);
            return json;
        }

        [HttpPost]
        public string UploadImage(string reference, long id, ICollection<IFormFile> file)
        {
            var context = new Context(files: file);
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
                guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents?.Length ?? 0);
            if (file == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            if (Parameters.BinaryStorage.IsContentPreviewable(file.ContentType))
            {
                var result = file.FileStream();
                return File(result.FileContents, result.ContentType);
            }
            return FileContentResults.FileStreamResult(file: file);
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
            log.Finish(
                context: context,
                responseSize: result?.FileContents?.Length ?? 0);
            if (result == null)
            {
                return null;
            }
            if (Parameters.BinaryStorage.IsContentPreviewable(result.ContentType))
            {
                return File(result.FileContents, result.ContentType);
            }
            return FileContentResults.FileStreamResult(file: file);
        }

        [HttpPost]
        public string DeleteTemp(string reference, long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.DeleteTemp(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json.ToString();
        }

        [HttpPost]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public ActionResult Upload(long id)
        {
            var files = Request.Form.Files;
            var context = new Context(files: files.ToList());
            var log = new SysLogModel(context: context);
            var contentRangeHeader = Request.Headers["Content-Range"].FirstOrDefault();
            var contentRange = BinaryUtilities.GetContentRange(contentRangeHeader: contentRangeHeader);
            var content= context.Authenticated
                ? BinaryUtilities.UploadFile(context, id, contentRange)
                : Messages.ResponseAuthentication(context: context).ToJson();
            log.Finish(context: context, responseSize: content.Length);
            return new ContentResult()
            {
                Content = content,
                ContentType = "application/json",
            };
        }
    }
}