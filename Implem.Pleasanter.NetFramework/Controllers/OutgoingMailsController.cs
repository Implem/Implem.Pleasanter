using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Controllers
{
    [Authorize]
    [ValidateInput(false)]
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
