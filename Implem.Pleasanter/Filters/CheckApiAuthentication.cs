using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.DataSources;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Server;
using Implem.Pleasanter.Models;
using System.Web.Mvc;
namespace Implem.Pleasanter.Filters
{
    public class CheckApiAuthentication : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var context = new Context();
            var api = Forms.String().Deserialize<Api>();
            if (api?.ApiKey.IsNullOrEmpty() == false)
            {
                var userModel = new UserModel().Get(
                    context: context,
                    ss: null,
                    where: Rds.UsersWhere()
                        .ApiKey(api.ApiKey)
                        .Disabled(0));
                if (userModel.AccessStatus != Databases.AccessStatuses.Selected)
                {
                    filterContext.Result = ApiResults.Unauthorized();
                }
                else
                {
                    userModel.SetSession();
                    if (context.ContractSettings.Api == false)
                    {
                        Sessions.Abandon();
                        filterContext.Result = ApiResults.BadRequest();
                    }
                }
            }
            else if (!Sessions.LoggedIn())
            {
                filterContext.Result = ApiResults.Unauthorized();
            }
        }
    }
}