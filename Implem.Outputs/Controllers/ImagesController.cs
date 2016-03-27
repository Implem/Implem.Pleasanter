using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class ImagesController : Controller
    {
        [HttpGet]
        public ActionResult Show(long id)
        {
            var log = new SysLogModel();
            var image = new ImageModel(id).Show();
            log.Finish(image.Length);
            return new FileContentResult(image, "image/jpeg");
        }

        [HttpGet]
        [OutputCache(Duration = 86400)]
        public ActionResult NavSiteThumbnail(string reference, long id)
        {
            var log = new SysLogModel();
            var image = ImagesUtility.NavSiteThumbnail(id);
            log.Finish(image.Length);
            return new FileContentResult(image, "image/png");
        }

        [HttpGet]
        [OutputCache(Duration = 86400)]
        public ActionResult NavSiteIcon(string reference, long id)
        {
            var log = new SysLogModel();
            var image = ImagesUtility.NavSiteIcon(id);
            log.Finish(image.Length);
            return new FileContentResult(image, "image/png");
        }

        [HttpPost]
        public string Update(string reference, long id)
        {
            var log = new SysLogModel();
            var json = ImagesUtility.Update(id);
            log.Finish(0);
            return json;
        }
    }
}