using Implem.Pleasanter.Libraries.General;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
namespace Implem.PleasanterFilters
{
    public class HandleErrorExAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var context = new Context(sessionData: false);
            if (filterContext == null)
            {
                throw new ArgumentNullException("No ExceptionContext");
            }
            try
            {
                new SysLogModel(
                    context: context,
                    method: nameof(OnException),
                    message: $"{filterContext.Exception.GetType().Name}: {filterContext.Exception.Message}",
                    errStackTrace: filterContext.Exception.ToString(),
                    sysLogType: SysLogModel.SysLogTypes.Exception);
                var siteId = CanManageSiteId(context: context);
                SessionUtilities.Set(
                    context: context,
                    key: "ExceptionSiteId",
                    value: siteId.ToString());
            }
            catch
            {
            }
            filterContext.ExceptionHandled = true;
            if (context.Ajax)
            {
                filterContext.Result = new ContentResult()
                {
                    Content = Error.Types.ApplicationError.MessageJson(context: context),
                    StatusCode = 500
                };
            }
            else
            {
                filterContext.Result = new RedirectResult(Locations.ApplicationError(context: context));
            }
        }

        private static long CanManageSiteId(Context context)
        {
            if (context.SiteId == 0)
            {
                return 0;
            }
            else
            {
                var ss = SiteSettingsUtilities.Get(
                    context: context,
                    siteId: context.SiteId);
                var siteId = context.CanManageSite(
                    ss: ss,
                    site: true)
                        ? context.SiteId
                        : 0;
                return siteId;
            }
        }
    }
}
