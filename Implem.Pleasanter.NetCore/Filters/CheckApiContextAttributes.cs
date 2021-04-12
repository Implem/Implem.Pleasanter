using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Security;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.IO.Pipelines;
using System.Text;
using Implem.Pleasanter.NetCore.Libraries.Requests;
using System.IO;

namespace Implem.Pleasanter.NetCore.Filters
{
    public class CheckApiContextAttributes : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (Parameters.Security.TokenCheck
                && filterContext.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                if (filterContext.HttpContext.Request?.Body == null)
                {
                    filterContext.Result = new BadRequestResult();
                    return;
                }
                var reader = new StreamReader(
                    stream: filterContext.HttpContext.Request?.Body,
                    encoding: Encoding.UTF8);
                var data = reader.ReadToEnd();
                var api = data?.Deserialize<Api>();
                if (api?.Token != ContextImplement.StaticToken())
                {
                    filterContext.Result = new BadRequestResult();
                }

            }
        }

    }
}