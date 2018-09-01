using Implem.Pleasanter.Libraries.Requests;
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
            var context = new Context();
            if (Sessions.LoggedIn() && context.ContractSettings.OverDeadline())
            {
                Authentications.SignOut();
                filterContext.Result = new RedirectResult(Locations.Login() + "?expired=1");
            }
        }
    }
}