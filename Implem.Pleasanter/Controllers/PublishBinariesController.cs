using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
using System.Web.UI;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    [Publishes]
    public class PublishBinariesController : Controller
    {
        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult SiteImageThumbnail(string reference, long id)
        {
            var context = new Context();
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

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult SiteImageIcon(string reference, long id)
        {
            var context = new Context();
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

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult TenantImageLogo(string reference, long id)
        {
            var context = new Context();
            if (reference.ToLower() == "tenants")
            {
                var bytes = BinaryUtilities.TenantImageLogo(
                    context: context,
                    tenantModel: new TenantModel(context: context,ss: SiteSettingsUtilities.TenantsSiteSettings(context)));
                return new FileContentResult(bytes, "image/png");
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        public FileContentResult Download(string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.Donwload(context: context, guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents.Length ?? 0);
            return file;
        }

        [HttpGet]
        public ActionResult Show(string guid)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var file = BinaryUtilities.Donwload(context: context, guid: guid);
            log.Finish(context: context, responseSize: file?.FileContents.Length ?? 0);
            return file != null
                ? File(file.FileContents, file.ContentType)
                : null;
        }
    }
}