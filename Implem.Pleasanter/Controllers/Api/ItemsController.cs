using System;
using System.Web.Http;

namespace Implem.Pleasanter.Controllers.Api
{
    public class ItemsController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Json(new { Path = "GET:api/Items", Env = new { Environment.MachineName, Environment.OSVersion } });
        }
    }
}
