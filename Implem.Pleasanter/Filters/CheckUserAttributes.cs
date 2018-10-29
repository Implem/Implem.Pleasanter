using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using System.Web.Mvc;
namespace Implem.Pleasanter.Filters
{
    public class CheckUserAttributes : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var context = new Context(
                routeProperties: false,
                sessionStatus: false,
                sessionData: false);
            if (!context.LoginId.IsNullOrEmpty())
            {
                if (!context.Authenticated)
                {
                    if (Authentications.Windows())
                    {
                        filterContext.Result = new EmptyResult();
                    }
                    else
                    {
                        Authentications.SignOut();
                        filterContext.Result = new RedirectResult(Locations.Login());
                    }
                }
            }
        }
    }
}