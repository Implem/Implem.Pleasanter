using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using Implem.Pleasanter.Libraries.Server;
using System.Web.Mvc;
namespace Implem.Pleasanter.Filters
{
    public class CheckContractAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Sessions.LoggedIn() && Contract.OverDeadline())
            {
                Authentications.SignOut();
                filterContext.Result = new RedirectResult(Locations.Login() + "?expired=1");
            }
        }
    }
}