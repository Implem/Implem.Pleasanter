using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    public class Api_DeptsController : Controller
    {
        [HttpPost]
        public ContentResult Get(int id = 0)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? DeptUtilities.GetByApi(
                    context: context,
                    ss: SiteSettingsUtilities.ApiDeptsSiteSettings(context),
                    deptId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}