using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers.Api
{
    public class DeptsController
    {
        public ContentResult Get(Context context)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new DeptModel().GetByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}