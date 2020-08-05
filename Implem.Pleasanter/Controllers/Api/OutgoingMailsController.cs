using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Implem.Pleasanter.Controllers.Api
{
    public class OutgoingMailsController
    {
        public ContentResult Send(Context context, string reference, long id)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? OutgoingMailUtilities.SendByApi(
                    context: context,
                    reference: reference,
                    id: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}