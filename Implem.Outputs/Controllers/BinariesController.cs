using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class BinariesController : Controller
    {
        [HttpGet]
        public ActionResult Show(long id)
        {
            var log = new SysLogModel();
            var image = new BinaryModel(id).Show();
            log.Finish(image.Length);
            return new FileContentResult(image, "image/jpeg");
        }

        [HttpGet]
        [OutputCache(Duration = 86400)]
        public ActionResult NavSiteThumbnail(string reference, long id)
        {
            var log = new SysLogModel();
            var image = BinariesUtility.NavSiteThumbnail(id);
            log.Finish(image.Length);
            return new FileContentResult(image, "image/png");
        }

        [HttpGet]
        [OutputCache(Duration = 86400)]
        public ActionResult NavSiteIcon(string reference, long id)
        {
            var log = new SysLogModel();
            var image = BinariesUtility.NavSiteIcon(id);
            log.Finish(image.Length);
            return new FileContentResult(image, "image/png");
        }

        [HttpPost]
        public string Update(string reference, long id)
        {
            var log = new SysLogModel();
            var json = BinariesUtility.Update(id);
            log.Finish(0);
            return json;
        }
    }
}