using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers.Api
{
    public class SessionsController
    {
        public ContentResult Get(Context context)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? SessionUtilities.GetByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        public ContentResult Set(Context context)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? SessionUtilities.SetByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        public ContentResult Delete(Context context)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? SessionUtilities.DeleteByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}
