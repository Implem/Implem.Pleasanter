using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class BinariesController : Controller
    {
        [HttpGet]
        [OutputCache(Duration = 86400)]
        public ActionResult SiteImageThumbnail(string reference, long id)
        {
            if (reference.ToLower() == "items")
            {
                var bytes = new BinaryModel(new SiteModel(id)).SiteImageThumbnail();
                return new FileContentResult(bytes, "image/png");
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        [OutputCache(Duration = 86400)]
        public ActionResult SiteImageIcon(string reference, long id)
        {
            if (reference.ToLower() == "items")
            {
                var bytes = new BinaryModel(new SiteModel(id)).SiteImageIcon();
                return new FileContentResult(bytes, "image/png");
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public string UpdateSiteImage(string reference, long id)
        {
            var log = new SysLogModel();
            var json = reference.ToLower() == "items"
                ? new BinaryModel(new SiteModel(id)).UpdateSiteImage()
                : new ResponseCollection().ToJson();
            log.Finish(0);
            return json;
        }
    }
}