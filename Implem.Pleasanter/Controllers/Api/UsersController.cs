using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Implem.Pleasanter.Controllers.Api
{
    public class UsersController
    {
        public ContentResult Get(Context context)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new UserModel().GetByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
        
        public ContentResult Create(Context context)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new UserModel().CreateByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        public ContentResult Update(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new UserModel().UpdateByApi(context: context, userId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        public ContentResult Delete(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new UserModel().DeleteByApi(context: context, userId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}