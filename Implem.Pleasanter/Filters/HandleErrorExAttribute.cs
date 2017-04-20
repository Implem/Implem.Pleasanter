using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using System;
using System.Web.Mvc;
using System.Web.SessionState;
namespace Implem.Pleasanter.Filters
{
    public class HandleErrorExAttribute : FilterAttribute, IExceptionFilter, IRequiresSessionState
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("No ExceptionContext");
            }
            try
            {
                filterContext.HttpContext.Session["Error"] = filterContext;
                new SysLogModel(filterContext);
            }
            catch
            {
                throw;
            }
            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectResult(Locations.ApplicationError());
        }
    }
}
