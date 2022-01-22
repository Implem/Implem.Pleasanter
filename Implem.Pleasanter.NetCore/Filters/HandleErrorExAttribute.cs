using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
namespace Implem.Pleasanter.NetCore.Filters
{
    public class HandleErrorExAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var context = new ContextImplement(sessionData: false);
            if (filterContext == null)
            {
                throw new ArgumentNullException("No ExceptionContext");
            }
            try
            {
                new SysLogModel(
                    context: context,
                    method: nameof(OnException),
                    message: filterContext.Exception.Message,
                    errStackTrace: filterContext.Exception.StackTrace,
                    sysLogType: SysLogModel.SysLogTypes.Execption);
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
            filterContext.Result = new RedirectResult(
                Locations.ApplicationError(context: context));
        }

        private static long CanManageSiteId(ContextImplement context)
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
