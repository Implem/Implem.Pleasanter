using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    [CheckApiAuthentication]
    public class Api_ItemsController : Controller
    {
        [HttpPost]
        public ContentResult Get(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = new ItemModel(context: context, referenceId: id)
                .GetByApi(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        [HttpPost]
        public ContentResult Create(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = new ItemModel(context: context, referenceId: id)
                .CreateByApi(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        [HttpPost]
        public ContentResult Update(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = new ItemModel(context: context, referenceId: id)
                .UpdateByApi(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        [HttpPost]
        public ContentResult Delete(long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = new ItemModel(context: context, referenceId: id)
                .DeleteByApi(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}