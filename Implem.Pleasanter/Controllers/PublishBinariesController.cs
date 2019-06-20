using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class PublishBinariesController
    {
        public FileContentResult SiteImageThumbnail(Context context, string reference, long id)
        {
            if (reference.ToLower() == "items")
            {
                var bytes = BinaryUtilities.SiteImageThumbnail(
                    context: context,
                    siteModel: new SiteModel(context: context, siteId: id));
                return new FileContentResult(bytes, "image/png");
            }
            else
            {
                return null;
            }
        }

        public FileContentResult SiteImageIcon(Context context, string reference, long id)
        {
            if (reference.ToLower() == "items")
            {
                var bytes = BinaryUtilities.SiteImageIcon(
                    context: context,
                    siteModel: new SiteModel(context: context, siteId: id));
                return new FileContentResult(bytes, "image/png");
            }
            else
            {
                return null;
            }
        }

        public FileContentResult TenantImageLogo(Context context)
        {
            var log = new SysLogModel(context: context);
            var bytes = BinaryUtilities.TenantImageLogo(
                context: context,
                tenantModel: new TenantModel(
                    context: context,
                    ss: SiteSettingsUtilities.TenantsSiteSettings(context)));
            log.Finish(
                context: context,
                responseSize: bytes.Length);
            return new FileContentResult(bytes, "image/png");
        }

        public FileContentResult Download(Context context, string guid)
        {
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.Donwload(context: context, guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents.Length ?? 0);
            return file;
        }

        public FileContentResult Show(Context context, string guid)
        {
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.Donwload(context: context, guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents.Length ?? 0);
            return file;
        }
    }
}