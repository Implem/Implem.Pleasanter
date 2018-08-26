using Implem.Pleasanter.Filters;
using Implem.Pleasanter.Libraries.Requests;
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
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = OutgoingMailUtilities.Editor(
                context: context,
                reference: reference,
                id: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPut]
        public string Reply(string reference, long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = OutgoingMailUtilities.Editor(
                context: context,
                reference: reference,
                id: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPut]
        public string GetDestinations(string reference, long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = new OutgoingMailModel(
                context: context, 
                reference: reference,
                referenceId: id)
                    .GetDestinations(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        [HttpPost]
        public string Send(string reference, long id)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var json = OutgoingMailUtilities.Send(
                context: context,
                reference: reference,
                id: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }
    }
}
