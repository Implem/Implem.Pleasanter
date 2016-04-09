using Implem.DefinitionAccessor;
using Implem.Pleasanter.Filters;
using System.Web.Mvc;
namespace Implem.Pleasanter
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new HandleErrorExAttribute());
            filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            if (Def.Parameters.RequireHttps == 1)
            {
                filters.Add(new RequireHttpsAttribute());
            }
        }
    }
}
