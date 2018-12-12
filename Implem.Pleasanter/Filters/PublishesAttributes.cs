using Implem.Pleasanter.Libraries.DataSources;
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
            if (!Rds.ExecuteScalar_bool(
                context: context,
                statements: Rds.SelectSites(
                    column: Rds.SitesColumn().Publish(),
                    where: Rds.SitesWhere()
                        .SiteId(sub: Rds.SelectItems(
                            column: Rds.ItemsColumn().SiteId(),
                            where: Rds.ItemsWhere().ReferenceId(context.Id))))))
            {
                filterContext.Result = new RedirectResult(Locations.BadRequest(context: context));
            }
        }
    }
}