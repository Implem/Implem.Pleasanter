using Implem.Pleasanter.Libraries.ServerData;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [Authorize]
    [ValidateInput(false)]
    public class OutgoingMailsController : Controller
    {
        [HttpPut]
        public string Edit(string reference, long id)
        {
            var log = new SysLogModel();
            var json = OutgoingMailsUtility.Editor(reference, id);
            log.Finish(json.Length);
            return json;
        }

        [HttpPut]
        public string Reply(string reference, long id)
        {
            var log = new SysLogModel();
            var json = OutgoingMailsUtility.Editor(reference, id);
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
            var json = new OutgoingMailModel(reference, id).Send();
            log.Finish(json.Length);
            return json;
        }
    }
}
