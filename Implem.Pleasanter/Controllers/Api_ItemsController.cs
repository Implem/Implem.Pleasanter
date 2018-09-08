using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    public class Api_ItemsController : Controller
    {
        [HttpPost]
        public ContentResult Get(long id)
        {
            var context = new Context(api: true);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new ItemModel(context: context, referenceId: id).GetByApi(context: context)
                : ApiResults.Unauthorized();
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        [HttpPost]
        public ContentResult Create(long id)
        {
            var context = new Context(api: true);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new ItemModel(context: context, referenceId: id).CreateByApi(context: context)
                : ApiResults.Unauthorized();
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        [HttpPost]
        public ContentResult Update(long id)
        {
            var context = new Context(api: true);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new ItemModel(context: context, referenceId: id).UpdateByApi(context: context)
                : ApiResults.Unauthorized();
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        [HttpPost]
        public ContentResult Delete(long id)
        {
            var context = new Context(api: true);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new ItemModel(context: context, referenceId: id).DeleteByApi(context: context)
                : ApiResults.Unauthorized();
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}