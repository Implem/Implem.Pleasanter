using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Models;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    exceptionMessage: filterContext.Exception.Message,
                    exceptionStackTrace: filterContext.Exception.StackTrace);
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
