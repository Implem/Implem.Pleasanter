using Implem.DefinitionAccessor;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class DemosController
    {
        public string Register(Context context)
        {
            var log = new SysLogModel(context: context);
            if (Parameters.Service.Demo)
            {
                var json = DemoUtilities.Register(context: context);
                log.Finish(context: context, responseSize: json.Length);
                return json;
            }
            else
            {
                log.Finish(context: context);
                return null;
            }
        }

        public (string redirectUrl, string errors, string notFound) Login(Context context)
        {
            var log = new SysLogModel(context: context);
            if (Parameters.Service.Demo)
            {
                DemoUtilities.Login(context: context);
                log.Finish(context: context);
                return (Locations.Get(context: context), null, null);
            }
            else
            {
                log.Finish(context: context);
                return (null, "Errors", "NotFound");
            }
        }
    }
}
