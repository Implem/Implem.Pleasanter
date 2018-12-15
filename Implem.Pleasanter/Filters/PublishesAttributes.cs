using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using System.Web.Mvc;
namespace Implem.Pleasanter.Filters
{
    public class Publishes : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var context = new Context(tenantId: 0);
            if (!context.Publish)
            {
                filterContext.Result = new RedirectResult(Locations.BadRequest(context: context));
            }
        }
    }
}