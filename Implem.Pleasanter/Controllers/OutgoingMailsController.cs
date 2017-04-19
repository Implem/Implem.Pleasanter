using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [CheckContract]
    [ValidateInput(false)]
    [RefleshSiteInfo]
    public class OutgoingMailsController : Controller
    {
        [HttpPut]
        public string Edit(string reference, long id)
        {
            var log = new SysLogModel();
            var json = OutgoingMailUtilities.Editor(reference, id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Reply(string reference, long id)
        {
            var log = new SysLogModel();
            var json = OutgoingMailUtilities.Editor(reference, id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string GetDestinations(string reference, long id)
        {
            var log = new SysLogModel();
            var json = new OutgoingMailModel(reference, id).GetDestinations();
            log.Finish(json.Length);
            return json;
        }

        [HttpPost]
        public string Send(string reference, long id)
        {
            var log = new SysLogModel();
            var json = OutgoingMailUtilities.Send(reference, id);
            log.Finish(json.Length);
            return json;
        }
    }
}
