using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [AllowAnonymous]
    public class Api_ItemsController : Controller
    {
        [HttpPost]
        public ContentResult Get(long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.Api_ItemsController();
            var result = controller.Get(context: context, id: id);
            return result;
        }

        [HttpPost]
        public ContentResult Create(long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.Api_ItemsController();
            var result = controller.Create(context: context, id: id);
            return result;
        }

        [HttpPost]
        public ContentResult Update(long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.Api_ItemsController();
            var result = controller.Update(context: context, id: id);
            return result;
        }

        [HttpPost]
        public ContentResult Delete(long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.Api_ItemsController();
            var result = controller.Delete(context: context, id: id);
            return result;
        }
    }
}