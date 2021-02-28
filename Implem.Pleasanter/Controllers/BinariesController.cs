﻿using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class BinariesController : Controller
    {
        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
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
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
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
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
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
        public string UpdateSiteImage(string reference, long id, HttpPostedFileBase[] file)
        {
            var context = new Context(files: file);
            var log = new SysLogModel(context: context);
            var json = reference.ToLower() == "items"
                ? BinaryUtilities.UpdateSiteImage(
                    context: context,
                    siteModel: new SiteModel(context: context, siteId: id))
                : new ResponseCollection().ToJson();
            log.Finish(context: context);
            return json;
        }

        /// <summary>
        /// Fixed:
        /// </summary>
        [HttpPost]
        public string UpdateTenantImage(HttpPostedFileBase[] file)
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
                : new ResponseCollection().ToJson();
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
        public string UploadImage(string reference, long id, HttpPostedFileBase[] file)
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

        [HttpPost]
        public string MultiUpload(string reference, long id, HttpPostedFileBase[] file)
        {
            var context = new Context(files: file);
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.MultiUpload(
                context: context,
                id: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpGet]
        public ActionResult Download(string reference, string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.Donwload(context: context, guid: guid);
            if (file == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            log.Finish(context: context, responseSize: file?.FileContents.Length ?? 0);
            return file;
        }

        [HttpGet]
        public ActionResult DownloadTemp(string reference, string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.DownloadTemp(context: context, guid: guid);
            if (file == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            log.Finish(context: context, responseSize: file?.FileContents.Length ?? 0);
            return file;
        }

        [HttpGet]
        public ActionResult Show(string reference, string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.Donwload(context: context, guid: guid);
            if (file == null)
            {
                return RedirectToAction("notfound", "errors");
            }
            log.Finish(context: context, responseSize: file?.FileContents.Length ?? 0);
            return file != null
                ? File(file.FileContents, file.ContentType)
                : null;
        }

        [HttpGet]
        public ActionResult ShowTemp(string reference, string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.DownloadTemp(context: context, guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents.Length ?? 0);
            return file != null
                ? File(file.FileContents, file.ContentType)
                : null;
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
    }
}