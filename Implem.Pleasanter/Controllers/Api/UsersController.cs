using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Controllers.Api
{
    public class UsersController
    {
        public ContentResult Get(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? UserUtilities.GetByApi(
                    context: context,
                    ss: SiteSettingsUtilities.ApiUsersSiteSettings(context),
                    userId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
        
        public ContentResult Create(Context context)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? UserUtilities.CreateByApi(
                    context: context,
                    ss: SiteSettingsUtilities.ApiUsersSiteSettings(context))
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        public ContentResult Update(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? UserUtilities.UpdateByApi(
                    context: context,
                    ss: SiteSettingsUtilities.ApiUsersSiteSettings(context),
                    userId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }

        public ContentResult Delete(Context context, int id)
        {
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? UserUtilities.DeleteByApi(
                    context: context,
                    ss: SiteSettingsUtilities.ApiUsersSiteSettings(context),
                    userId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result;
        }
    }
}