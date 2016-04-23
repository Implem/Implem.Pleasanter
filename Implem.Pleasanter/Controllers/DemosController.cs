using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class DemosController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public string Register()
        {
            var log = new SysLogModel();
            var json = DemosUtility.Register();
            log.Finish(json.Length);
            return json;
        }
    }
}
