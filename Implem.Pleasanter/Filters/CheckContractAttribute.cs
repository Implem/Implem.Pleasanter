using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Security;
using System.Web.Mvc;
namespace Implem.Pleasanter.Filters
{
    public class CheckContractAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var context = new Context();
            if (context.Authenticated
                && !context.ContractSettings.AllowedIpAddress(context.UserHostAddress))
            {
                Authentications.SignOut();
                filterContext.Result = new RedirectResult(Locations.BadRequest());
                return;
            }
            if (context.Authenticated && context.ContractSettings.OverDeadline(context: context))
            {
                Authentications.SignOut();
                filterContext.Result = new RedirectResult(Locations.Login() + "?expired=1");
            }
        }
    }
}