using Implem.Pleasanter.Libraries.Requests;
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
            var result = context.Authenticated
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
            var result = context.Authenticated
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
            var result = context.Authenticated
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

        //[HttpPost("{id}/Delete")]
        //public ContentResult Delete(int id)
        //{
        //    var body = default(string);
        //    using (var reader = new StreamReader(Request.Body)) body = reader.ReadToEnd();
        //    var context = new Context(
        //        sessionStatus: User?.Identity?.IsAuthenticated == true,
        //        sessionData: User?.Identity?.IsAuthenticated == true,
        //        apiRequestBody: body,
        //        contentType: Request.ContentType,
        //        api: true);
        //    var log = new SysLogModel(context: context);
        //    var result = context.Authenticated
        //        ? GroupUtilities.DeleteByApi(
        //            context: context,
        //            ss: SiteSettingsUtilities.ApiGroupsSiteSettings(context),
        //            groupId: id)
        //        : ApiResults.Unauthorized(context: context);
        //    log.Finish(
        //        context: context,
        //        responseSize: result.Content.Length);
        //    return result.ToHttpResponse(request: Request);
        //}

        //[HttpPost("Import")]
        //public ContentResult Import(int id)
        //{
        //    var body = Request.Form["parameters"];
        //    var contentType = Request.ContentType.Split(';')[0].Trim();
        //    var context = new Context(
        //        apiRequestBody: body,
        //        files: Request.Form.Files.ToList(),
        //        contentType: contentType,
        //        api: true);
        //    var log = new SysLogModel(context: context);
        //    var result = context.Authenticated
        //        ? GroupUtilities.ImportByApi(
        //            context: context,
        //            ss: SiteSettingsUtilities.ApiGroupsSiteSettings(context))
        //        : ApiResults.Unauthorized(context: context);
        //    log.Finish(context: context, responseSize: result.Content.Length);
        //    return result.ToHttpResponse(request: Request);
        //}
    }
}