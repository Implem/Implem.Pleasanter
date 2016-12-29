using Implem.Pleasanter.Libraries.Server;
using System.Web.Mvc;
namespace Implem.Pleasanter.Filters
{
    public class RefleshSiteInfoAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            SiteInfo.Reflesh();
        }
    }
}