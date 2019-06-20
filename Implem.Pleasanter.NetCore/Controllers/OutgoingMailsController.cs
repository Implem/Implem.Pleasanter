using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Implem.Pleasanter.NetCore.Controllers
{
    [Authorize]
    public class OutgoingMailsController : Controller
    {
        [HttpPut]
        public string Edit(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.OutgoingMailsController();
            var json = controller.Edit(context: context, reference: reference, id: id);
            return json;
        }

        [HttpPut]
        public string Reply(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.OutgoingMailsController();
            var json = controller.Reply(context: context, reference: reference, id: id);
            return json;
        }

        [HttpPut]
        public string GetDestinations(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.OutgoingMailsController();
            var json = controller.GetDestinations(context: context, reference: reference, id: id);
            return json;
        }

        [HttpPost]
        public string Send(string reference, long id)
        {
            var context = new ContextImplement();
            var controller = new Implem.Pleasanter.Controllers.OutgoingMailsController();
            var json = controller.Send(context: context, reference: reference, id: id);
            return json;
        }
    }
}
