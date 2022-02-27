using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Implem.PleasanterFilters
{
    public class PublishesAttributes : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var context = new Context(tenantId: 0);
            if (!context.Publish)
            {
                filterContext.Result = new RedirectResult(Locations.BadRequest(context: context));
            }
        }
    }
}
