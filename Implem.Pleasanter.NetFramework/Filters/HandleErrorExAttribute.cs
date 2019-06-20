using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System;
using System.Web.Mvc;
using System.Web.SessionState;
namespace Implem.Pleasanter.NetFramework.Filters
{
    public class HandleErrorExAttribute : FilterAttribute, IExceptionFilter, IRequiresSessionState
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
                    filterContext: filterContext);
            }
            catch
            {
                throw;
            }
            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectResult(
                Locations.ApplicationError(context: context));
        }
    }
}
