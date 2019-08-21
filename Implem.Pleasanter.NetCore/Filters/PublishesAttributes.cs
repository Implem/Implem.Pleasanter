using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Implem.Pleasanter.NetCore.Filters
{
    public class PublishesAttributes : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var context = new ContextImplement(tenantId: 0);
            if (!context.Publish)
            {
                filterContext.Result = new RedirectResult(Locations.BadRequest(context: context));
            }
        }
    }
}
