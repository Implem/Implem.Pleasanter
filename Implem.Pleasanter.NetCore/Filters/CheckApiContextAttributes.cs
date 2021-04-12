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

namespace Implem.Pleasanter.NetCore.Filters
{
    public class CheckApiContextAttributes : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (Parameters.Security.TokenCheck
                && filterContext.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                var length = filterContext.HttpContext.Request?.Body?.Length ?? 0;
                if (length > 0)
                {
                    byte[] buffer = new byte[length];
                    filterContext.HttpContext.Request?.Body.Read(buffer);
                    var data = Encoding.UTF8.GetString(buffer);
                    var api = data?.Deserialize<Api>();
                    if (api?.Token != ContextImplement.StaticToken())
                    {
                        filterContext.Result = new BadRequestResult();
                    }
                }
            }
        }

    }
}