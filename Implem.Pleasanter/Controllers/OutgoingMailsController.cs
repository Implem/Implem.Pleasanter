using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers
{
    public class OutgoingMailsController
    {
        public string Edit(Context context, string reference, long id)
        {
            var log = new SysLogModel(context: context);
            var json = OutgoingMailUtilities.Editor(
                context: context,
                reference: reference,
                id: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Reply(Context context, string reference, long id)
        {
            var log = new SysLogModel(context: context);
            var json = OutgoingMailUtilities.Editor(
                context: context,
                reference: reference,
                id: id);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string GetDestinations(Context context, string reference, long id)
        {
            var log = new SysLogModel(context: context);
            var json = new OutgoingMailModel(
                context: context, 
                reference: reference,
                referenceId: id)
                    .GetDestinations(context: context);
            log.Finish(context: context, responseSize: json.Length);
            return json;
        }

        public string Send(Context context, string reference, long id)
        {
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
