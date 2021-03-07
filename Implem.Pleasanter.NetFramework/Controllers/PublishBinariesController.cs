using Implem.Pleasanter.NetFramework.Filters;
using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
using System.Web.UI;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [AllowAnonymous]
    [Publishes]
    public class PublishBinariesController : Controller
    {
        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult SiteImageThumbnail(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.PublishBinariesController();
            var file = controller.SiteImageThumbnail(context: context, reference: reference, id: id);
            return file;
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult SiteImageIcon(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.PublishBinariesController();
            var file = controller.SiteImageIcon(context: context, reference: reference, id: id);
            return file;
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ActionResult TenantImageLogo()
        {
            var context = new ContextImplement();
            var controller = new Pleasanter.Controllers.PublishBinariesController();
            var file = controller.TenantImageLogo(context: context);
            return file;
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
            return file;
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
            return File(file.FileContents, file.ContentType);
        }
    }
}