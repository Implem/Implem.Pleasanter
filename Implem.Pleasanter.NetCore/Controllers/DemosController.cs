using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Implem.Pleasanter.NetCore.Controllers
{
    [Authorize]
    public class DemosController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public string Register()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DemosController();
            var json = controller.Register(context: context);
            return json;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.DemosController();
            var (redirectUrl, errors, notFound) = controller.Login(context: context);
            if (!string.IsNullOrWhiteSpace(redirectUrl))
            {
                return Redirect(redirectUrl);
            }
            else
            {
                return RedirectToAction(errors, notFound);
            }
        }
    }
}
