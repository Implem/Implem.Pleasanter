using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class BinariesController
    {
        public ActionResult SiteImageThumbnail(Context context, string reference, long id)
        {
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

        public ActionResult SiteImageIcon(Context context, string reference, long id)
        {
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

        public ActionResult TenantImageLogo(Context context)
        {
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

        public string UpdateSiteImage(Context context, string reference, long id, IHttpPostedFile[] file)
        {
            var log = new SysLogModel(context: context);
            var json = reference.ToLower() == "items"
                ? BinaryUtilities.UpdateSiteImage(
                    context: context,
                    siteModel: new SiteModel(context: context, siteId: id))
                : new ResponseCollection().ToJson();
            log.Finish(context: context);
            return json;
        }

        public string UpdateTenantImage(Context context, IHttpPostedFile[] file)
        {
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context);
            var tenantModel = new TenantModel(context, ss).Get(context, ss);
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.UpdateTenantImage(
                context: context,
                tenantModel: tenantModel);
            log.Finish(context: context);
            return json;
        }

        public string DeleteSiteImage(Context context, string reference, long id)
        {
            var log = new SysLogModel(context: context);
            var json = reference.ToLower() == "items"
                ? BinaryUtilities.DeleteSiteImage(
                    context: context,
                    siteModel: new SiteModel(context: context, siteId: id))
                : new ResponseCollection().ToJson();
            log.Finish(context: context);
            return json;
        }

        public string DeleteTenantImage(Context context)
        {
            var ss = SiteSettingsUtilities.TenantsSiteSettings(context);
            var tenantModel = new TenantModel(context, ss).Get(context, ss);
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.DeleteTenantImage(
                context: context,
                tenantModel: tenantModel);
            log.Finish(context: context);
            return json;
        }

        public string UploadImage(Context context, string reference, long id, IHttpPostedFile[] file)
        {
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.UploadImage(
                context: context,
                id: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string DeleteImage(Context context, string reference, string guid)
        {
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.DeleteImage(context: context, guid: guid);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string MultiUpload(Context context, string reference, long id, IHttpPostedFile[] file)
        {
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.MultiUpload(
                context: context,
                id: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public FileResult Download(Context context, string reference, string guid)
        {
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.Donwload(context: context, guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents?.Length ?? 0);
            return file?.IsFileInfo() == true
                ? (FileResult)new FilePathResult(file?.FileInfo.FullName, file?.ContentType) { FileDownloadName = file?.FileDownloadName }
                : (FileResult)new FileStreamResult(file?.FileContentsStream, file?.ContentType) { FileDownloadName = file?.FileDownloadName };
        }

        public FileResult DownloadTemp(Context context, string reference, string guid)
        {
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.DownloadTemp(context: context, guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents?.Length ?? 0);
            return file?.IsFileInfo() == true
                ? (FileResult)new FilePathResult(file?.FileInfo.FullName, file?.ContentType) { FileDownloadName = file?.FileDownloadName }
                : (FileResult)new FileStreamResult(file?.FileContentsStream, file?.ContentType) { FileDownloadName = file?.FileDownloadName };
        }

        public FileContentResult Show(Context context, string reference, string guid)
        {
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.Donwload(
                context: context,
                guid: guid)
                    ?.FileStream();
            log.Finish(context: context, responseSize: file?.FileContents?.Length ?? 0);
            return file != null
                ? new FileContentResult(file.FileContents, file.ContentType)
                : null;
        }

        public FileContentResult ShowTemp(Context context, string reference, string guid)
        {
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.DownloadTemp(context: context, guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents?.Length ?? 0);
            return file != null
                ? new FileContentResult(System.Text.Encoding.UTF8.GetBytes(file.FileContents), file.ContentType)
                : null;
        }

        public string DeleteTemp(Context context, string reference, long id)
        {
            var log = new SysLogModel(context: context);
            var json = BinaryUtilities.DeleteTemp(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json.ToString();
        }

        public object Upload(Context context, long id)
        {
            
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? BinaryUtilities.UploadFile(context, id, HttpContext.Request.Files)
                : ApiResults.Unauthorized(context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}