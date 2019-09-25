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
            var controller = new Implem.Pleasanter.Controllers.PublishBinariesController();
            var file = controller.SiteImageThumbnail(context: context, reference: reference, id: id);
            return file.ToFileContentResult();
        }

        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ActionResult SiteImageIcon(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.PublishBinariesController();
            var file = controller.SiteImageIcon(context: context, reference: reference, id: id);
            return file.ToFileContentResult();
        }

        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ActionResult TenantImageLogo()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.PublishBinariesController();
            var file = controller.TenantImageLogo(context: context);
            return file.ToFileContentResult();
        }

        [HttpGet]
        public FileContentResult Download(string guid)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.PublishBinariesController();
            var file = controller.Download(context: context, guid: guid);
            return ((System.Web.Mvc.FileContentResult)file).ToFileContentResult();
        }

        [HttpGet]
        public ActionResult Show(string guid)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.PublishBinariesController();
            var file = controller.Show(context: context, guid: guid);
            return file != null
                ? File(file.FileContents, file.ContentType)
                : null;
        }
    }
}