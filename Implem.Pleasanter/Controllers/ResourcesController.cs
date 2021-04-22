using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Resources;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class ResourcesController
    {
        public ContentResult Scripts(Context context)
        {
            var log = new SysLogModel(context: context);
            var result = JavaScripts.Get(context: context);
            log.Finish(
                context: context,
                responseSize: result.Content.Length);
            return result;
        }

        public ContentResult Styles(Context context)
        {
            var log = new SysLogModel(context: context);
            var result = Css.Get(context: context);
            log.Finish(
                context: context,
                responseSize: result.Content.Length);
            return result;
        }
    }
}