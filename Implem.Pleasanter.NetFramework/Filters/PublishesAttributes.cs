using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.NetFramework.Libraries.Requests;
using System.Web.Mvc;
namespace Implem.Pleasanter.NetFramework.Filters
{
    public class Publishes : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var context = new ContextImplement(tenantId: 0);
            if (!context.Publish)
            {
                filterContext.Result = new RedirectResult(Locations.BadRequest(context: context));
            }
        }
    }
}