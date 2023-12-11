using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Web;
using Implem.Pleasanter.Models;
using Implem.PleasanterFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Implem.Pleasanter.Controllers.Api
{
    [CheckApiContextAttributes]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        [HttpPost("{id}/Get")]
        public ContentResult Get(long id)
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
                ? new ItemModel(context: context, referenceId: id).GetByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Create")]
        public ContentResult Create(long id)
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
                ? new ItemModel(context: context, referenceId: id).CreateByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Update")]
        public ContentResult Update(long id)
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
                ? new ItemModel(context: context, referenceId: id).UpdateByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Upsert")]
        public ContentResult Upsert(long id)
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
                ? new ItemModel(context: context, referenceId: id).UpsertByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Delete")]
        public ContentResult Delete(long id)
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
                ? new ItemModel(context: context, referenceId: id).DeleteByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/BulkDelete")]
        public ContentResult BulkDelete(long id)
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
                ? new ItemModel(context: context, referenceId: id).BulkDeleteByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Import")]
        public ContentResult Import(long id)
        {
            var body = Request.Form["parameters"];
            var contentType = Request.ContentType.Split(';')[0].Trim();
            var context = new Context(
                apiRequestBody: body,
                files: Request.Form.Files.ToList(),
                contentType: contentType,
                api: true);
            var log = new SysLogModel(context: context);
            var result = context.Authenticated
                ? new ItemModel(context: context, referenceId: id).ImportByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/Export")]
        public ContentResult Export(long id)
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
                ? new ItemModel(context: context, referenceId: id).ExportByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/GetSite")]
        public ContentResult GetSite(long id)
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
                ? new ItemModel(context: context, referenceId: id).GetByApi(
                    context: context,
                    referenceType: "Sites")
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/CreateSite")]
        public ContentResult CreateSite(long id)
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
                ? new ItemModel(context: context, referenceId: id).CreateByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/UpdateSite")]
        public ContentResult UpdateSite(long id)
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
                ? new ItemModel(context: context, referenceId: id).UpdateByApi(
                    context: context,
                    referenceType: "Sites")
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/DeleteSite")]
        public ContentResult DeleteSite(long id)
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
                ? new ItemModel(context: context, referenceId: id).DeleteByApi(
                    context: context,
                    referenceType: "Sites")
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/CopySitePackage")]
        public ContentResult CopySitePackage(long id)
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
                ? new ItemModel(context: context, referenceId: id).CopySitePackageByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }

        [HttpPost("{id}/SynchronizeSummaries")]
        public ContentResult SynchronizeSummaries(long id)
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
            if (!context.Authenticated)
            {
                return ApiResults.Unauthorized(context: context);
            }
            else
            {
                log.Finish(context: context, responseSize: 0);
                return new ItemModel(
                    context: context,
                    referenceId: id).SynchronizeSummariesByApi(context: context);
            }
        }

        [HttpPost("{id}/updatesitesettings")]
        public ContentResult UpdateSiteSettings(long id)
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
                ? new ItemModel(context: context, referenceId: id).UpdateSiteSettingsByApi(context: context)
                : ApiResults.Unauthorized(context: context);
            log.Finish(context: context, responseSize: result.Content.Length);
            return result.ToHttpResponse(request: Request);
        }
    }
}
