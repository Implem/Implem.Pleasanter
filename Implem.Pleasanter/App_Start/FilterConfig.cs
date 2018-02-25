﻿using Implem.DefinitionAccessor;
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
            filters.Add(new CheckParameterSyntaxError());
            filters.Add(new RequestLimitAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new CheckUserAttributes());
            if (Parameters.Service.RequireHttps)
            {
                filters.Add(new RequireHttpsAttribute());
            }
        }
    }
}
