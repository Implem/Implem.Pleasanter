using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Implem.PleasanterFilters
{
    public class FormsAttributes : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var context = new Context(tenantId: 0);
            if (!context.IsForm) 
            {
                filterContext.Result = new RedirectResult(Locations.BadRequest(context: context));
            }
        }
    }
}
