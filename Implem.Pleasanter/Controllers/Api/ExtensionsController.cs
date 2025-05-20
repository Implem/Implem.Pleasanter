﻿using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using Org.BouncyCastle.X509.Extension;

namespace Implem.Pleasanter.Controllers.Api
{
    [CheckApiContextAttributes]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ExtensionsController : ControllerBase
    {
        [HttpPost("Get")]
        [HttpPost("{id}/Get")]
        public ContentResult Get(int id = 0)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.ContentType,
                api: true);
            var log = new SysLogModel(context: context);
            if (!context.AllowExtensionsApi)
            {
                return ApiResults.Forbidden(context: context);
                
            }
            var result = context.Authenticated && context.HasPrivilege
                ? ExtensionUtilities.GetByApi(
                    context: context,
                    ss: SiteSettingsUtilities.ApiExtensionsSiteSettings(context),
                    extensionId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(
                context: context,
                responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("Create")]
        public ContentResult Create()
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.ContentType,
                api: true);
            var log = new SysLogModel(context: context);
            if (!context.AllowExtensionsApi)
            {
                return ApiResults.Forbidden(context: context);
            }
            var result = context.Authenticated && context.HasPrivilege
                ? ExtensionUtilities.CreateByApi(
                    context: context,
                    ss: SiteSettingsUtilities.ApiExtensionsSiteSettings(context))
                : ApiResults.Unauthorized(context: context);
            log.Finish(
                context: context,
                responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Update")]
        public ContentResult Update(int id)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.ContentType,
                api: true);
            var log = new SysLogModel(context: context);
            if (!context.AllowExtensionsApi)
            {
                return ApiResults.Forbidden(context: context);
            }
            var result = context.Authenticated && context.HasPrivilege
                ? ExtensionUtilities.UpdateByApi(
                    context: context,
                    ss: SiteSettingsUtilities.ApiExtensionsSiteSettings(context),
                    extensionId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(
                context: context,
                responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Delete")]
        public ContentResult Delete(int id)
        {
            var body = default(string);
            using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
            var context = new Context(
                sessionStatus: User?.Identity?.IsAuthenticated == true,
                sessionData: User?.Identity?.IsAuthenticated == true,
                apiRequestBody: body,
                contentType: Request.ContentType,
                api: true);
            var log = new SysLogModel(context: context);
            if (!context.AllowExtensionsApi)
            {
                return ApiResults.Forbidden(context: context);
            }
            var result = context.Authenticated && context.HasPrivilege
                ? ExtensionUtilities.DeleteByApi(
                    context: context,
                    ss: SiteSettingsUtilities.ApiExtensionsSiteSettings(context),
                    extensionId: id)
                : ApiResults.Unauthorized(context: context);
            log.Finish(
                context: context,
                responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

    }
}