using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    public class ResourcesController : Controller
    {
        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ContentResult Scripts()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = JavaScripts.Get(context: context);
            log.Finish(
                context: context,
                responseSize: result.Content.Length);
            return result.ToRecourceContentResult(request: Request);
        }

        [HttpGet]
        [ResponseCache(Duration = int.MaxValue)]
        public ContentResult Styles()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = Css.Get(context: context);
            log.Finish(
                context: context,
                responseSize: result.Content.Length);
            return result.ToRecourceContentResult(request: Request);
        }

        [HttpPost]
        [ResponseCache(Duration = int.MaxValue)]
        public string Responsive(string Responsive)
        {
            var context = new Context();
            var responsive = !Responsive.ToBool();
            SessionUtilities.Set(
                context: context,
                key: "Responsive",
                value: responsive.ToString());
            return new ResponseCollection().ToJson();
        }
    }
}