using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [CheckApiAuthentication]
    public class Api_ItemsController : Controller
    {
        [HttpPost]
        [AllowAnonymous]
        public ContentResult Create(long id)
        {
            var log = new SysLogModel();
            var result = new ItemModel(id).CreateByApi();
            log.Finish(result.Content.Length);
            return result;
        }
    }
}