using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    [ValidateInput(false)]
    public class ResourcesController : Controller
    {
        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ContentResult Scripts()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = JavaScripts.Get(context: context);
            log.Finish(
                context: context,
                responseSize: result.Content.Length);
            return result;
        }

        [HttpGet]
        [OutputCache(Duration = int.MaxValue, VaryByParam = "*", Location = OutputCacheLocation.Client)]
        public ContentResult Styles()
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = Css.Get(context: context);
            log.Finish(
                context: context,
                responseSize: result.Content.Length);
            return result;
        }

        [HttpPost]
        public string Responsive()
        {
            var context = new Context();
            var responsive = !(Request.Params.Get("Responsive")?.ToBool());
            SessionUtilities.Set(
                context: context,
                key: "Responsive",
                value: responsive?.ToString());

            return new ResponseCollection().ToJson();
        }
    }
}