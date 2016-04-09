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
            var responseCollection = OutgoingMailsUtility.Editor(reference, id);
            log.Finish(responseCollection.Length);
            return responseCollection;
        }

        [HttpPut]
        public string Reply(string reference, long id)
        {
            var log = new SysLogModel();
            var responseCollection = OutgoingMailsUtility.Editor(reference, id);
            log.Finish(responseCollection.Length);
            return responseCollection;
        }

        [HttpPut]
        public string GetDestinations(string reference, long id)
        {
            var log = new SysLogModel();
            var responseCollection = new OutgoingMailModel(reference, id).GetDestinations();
            log.Finish(responseCollection.Length);
            return responseCollection;
        }

        [HttpPost]
        public string Send(string reference, long id)
        {
            var log = new SysLogModel();
            var responseCollection = new OutgoingMailModel(reference, id).Send();
            log.Finish(responseCollection.Length);
            return responseCollection;
        }
    }
}
