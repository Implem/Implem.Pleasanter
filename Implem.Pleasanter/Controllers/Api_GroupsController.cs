﻿using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Implem.Pleasanter.Controllers
{
    [AllowAnonymous]
    public class Api_GroupsController : Controller
    {
        [HttpPost]
        public ContentResult Get(int id = 0)
        {
            var context = new Context();
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? GroupUtilities.GetByApi(
                    context: context,
                    ss: SiteSettingsUtilities.ApiGroupsSiteSettings(context),
                    groupId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }
    }
}